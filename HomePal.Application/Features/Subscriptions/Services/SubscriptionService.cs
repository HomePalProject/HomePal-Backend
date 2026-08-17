using System.Text.Json;
using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Subscriptions.DTOs;
using HomePal.Application.Features.Subscriptions.Interfaces;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;
using HomePal.Domain.Enums;
using HomePal.Shared.Results;
using Microsoft.Extensions.Logging;

namespace HomePal.Application.Features.Subscriptions.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymobService _paymobService;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(
        IUnitOfWork unitOfWork,
        IPaymobService paymobService,
        ILogger<SubscriptionService> logger)
    {
        _unitOfWork = unitOfWork;
        _paymobService = paymobService;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<SubscriptionPlanResponse>>> GetPlansAsync(CancellationToken cancellationToken = default)
    {
        var plans = await _unitOfWork.SubscriptionPlans.GetActivePlansAsync(cancellationToken);
        var dtos = plans.Select(p => new SubscriptionPlanResponse
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name.Get(),
            Description = p.Description?.Get(),
            Price = p.Price,
            Currency = p.Currency,
            DurationInDays = p.DurationInDays
        }).ToList();

        return Result<IReadOnlyList<SubscriptionPlanResponse>>.Ok(dtos, SuccessMessages.Subscriptions.GetPlans);
    }

    public async Task<Result<UserSubscriptionResponse>> GetCurrentUserSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var subscription = await _unitOfWork.UserSubscriptions.GetActiveSubscriptionByUserIdAsync(userId, cancellationToken);
        
        if (subscription == null)
        {
            var latest = await _unitOfWork.UserSubscriptions.GetLatestSubscriptionByUserIdAsync(userId, cancellationToken);
            if (latest != null)
            {
                return Result<UserSubscriptionResponse>.Ok(new UserSubscriptionResponse
                {
                    Id = latest.Id,
                    UserId = latest.UserId,
                    PlanId = latest.PlanId,
                    PlanName = latest.Plan?.Name?.Get(),
                    Status = latest.EndDate < DateTime.UtcNow ? SubscriptionStatus.Expired : latest.Status,
                    StartDate = latest.StartDate,
                    EndDate = latest.EndDate,
                    IsActive = false,
                    RemainingDays = 0
                }, SuccessMessages.Subscriptions.GetCurrentSubscription);
            }

            return Result<UserSubscriptionResponse>.Ok(new UserSubscriptionResponse
            {
                UserId = userId,
                Status = SubscriptionStatus.Inactive,
                IsActive = false,
                RemainingDays = 0
            }, SuccessMessages.Subscriptions.GetCurrentSubscription);
        }

        var response = new UserSubscriptionResponse
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            PlanId = subscription.PlanId,
            PlanName = subscription.Plan?.Name?.Get(),
            Status = subscription.Status,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            IsActive = subscription.IsActiveSubscription,
            RemainingDays = Math.Max(0, (int)(subscription.EndDate - DateTime.UtcNow).TotalDays)
        };

        return Result<UserSubscriptionResponse>.Ok(response, SuccessMessages.Subscriptions.GetCurrentSubscription);
    }

    public async Task<bool> HasActiveSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var subscription = await _unitOfWork.UserSubscriptions.GetActiveSubscriptionByUserIdAsync(userId, cancellationToken);
        return subscription != null && subscription.IsActiveSubscription;
    }

    public async Task<Result<InitiatePaymentResponse>> InitiatePaymentAsync(Guid userId, Guid planId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return Result<InitiatePaymentResponse>.Fail(ErrorMessages.Auth.UserNotFound, ResultStatus.NotFound);
        }

        var hasActiveSub = await HasActiveSubscriptionAsync(userId, cancellationToken);
        if (hasActiveSub)
        {
            return Result<InitiatePaymentResponse>.Fail(ErrorMessages.Subscriptions.AlreadySubscribed, ResultStatus.Conflict);
        }

        var plan = await _unitOfWork.SubscriptionPlans.GetByIdAsync(planId, cancellationToken);
        if (plan == null)
        {
            return Result<InitiatePaymentResponse>.Fail(ErrorMessages.Subscriptions.PlanNotFound, ResultStatus.NotFound);
        }

        if (!plan.IsActive)
        {
            return Result<InitiatePaymentResponse>.Fail(ErrorMessages.Subscriptions.PlanInactive, ResultStatus.NotFound);
        }

        var paymentTransaction = new PaymentTransaction
        {
            UserId = userId,
            PlanId = planId,
            Amount = plan.Price,
            Currency = plan.Currency,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.PaymentTransactions.AddAsync(paymentTransaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var merchantOrderId = $"HP_SUB_{paymentTransaction.Id:N}";
        var names = (user.FullName ?? "HomePal User").Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var firstName = names.Length > 0 ? names[0] : "HomePal";
        var lastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "User";
        var email = !string.IsNullOrWhiteSpace(user.Email) ? user.Email : "user@homepal.app";
        var phone = !string.IsNullOrWhiteSpace(user.PhoneNumber) ? user.PhoneNumber : "NA";

        try
        {
            var authToken = await _paymobService.GetAuthTokenAsync(cancellationToken);
            var orderId = await _paymobService.CreateOrderAsync(authToken, plan.Price, plan.Currency, merchantOrderId, cancellationToken);
            
            paymentTransaction.PaymobOrderId = orderId.ToString();
            _unitOfWork.PaymentTransactions.Update(paymentTransaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var paymentToken = await _paymobService.GeneratePaymentKeyAsync(
                authToken,
                orderId,
                plan.Price,
                plan.Currency,
                email,
                firstName,
                lastName,
                phone,
                cancellationToken);

            var iframeUrl = _paymobService.GetIframeUrl(paymentToken);

            return Result<InitiatePaymentResponse>.Ok(new InitiatePaymentResponse
            {
                PaymentToken = paymentToken,
                IframeUrl = iframeUrl,
                PaymobOrderId = orderId.ToString(),
                Amount = plan.Price,
                Currency = plan.Currency
            }, SuccessMessages.Subscriptions.InitiatePayment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initiate Paymob payment for User {UserId} and Plan {PlanId}", userId, planId);
            paymentTransaction.Status = PaymentStatus.Failed;
            _unitOfWork.PaymentTransactions.Update(paymentTransaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<InitiatePaymentResponse>.Fail(ErrorMessages.Subscriptions.PaymentInitiationFailed, ResultStatus.ServiceUnavailable);
        }
    }

    public async Task<Result> ProcessPaymobWebhookAsync(PaymobWebhookPayload payload, CancellationToken cancellationToken = default)
    {
        if (payload?.Obj == null)
        {
            _logger.LogWarning("Received empty or invalid Paymob webhook payload.");
            return Result.Fail(ErrorMessages.Subscriptions.InvalidWebhookPayload, ResultStatus.BadRequest);
        }

        var transactionObj = payload.Obj;

        // Verify HMAC if provided
        if (!string.IsNullOrWhiteSpace(payload.Hmac))
        {
            var isHmacValid = _paymobService.VerifyHmac(transactionObj, payload.Hmac);
            if (!isHmacValid)
            {
                _logger.LogWarning("Paymob HMAC signature mismatch for transaction ID {TxnId}", transactionObj.Id);
                return Result.Fail(ErrorMessages.Subscriptions.HmacVerificationFailed, ResultStatus.Unauthorized);
            }
        }

        var paymobOrderId = transactionObj.Order?.Id.ToString();
        if (string.IsNullOrWhiteSpace(paymobOrderId))
        {
            return Result.Fail(ErrorMessages.Subscriptions.OrderIdMissing, ResultStatus.BadRequest);
        }

        var paymentTransaction = await _unitOfWork.PaymentTransactions.GetByPaymobOrderIdAsync(paymobOrderId, cancellationToken);
        if (paymentTransaction == null)
        {
            _logger.LogWarning("Payment transaction not found for Paymob Order ID {OrderId}", paymobOrderId);
            return Result.Fail(ErrorMessages.Subscriptions.TransactionNotFound, ResultStatus.NotFound);
        }

        paymentTransaction.PaymobTransactionId = transactionObj.Id.ToString();
        paymentTransaction.PaymentMethod = transactionObj.SourceData?.SubType ?? transactionObj.SourceData?.Type;
        paymentTransaction.RawCallbackData = JsonSerializer.Serialize(payload);
        paymentTransaction.UpdatedAt = DateTime.UtcNow;

        if (transactionObj.Success)
        {
            paymentTransaction.Status = PaymentStatus.Success;

            var plan = paymentTransaction.Plan ?? (paymentTransaction.PlanId.HasValue
                ? await _unitOfWork.SubscriptionPlans.GetByIdAsync(paymentTransaction.PlanId.Value, cancellationToken)
                : null);

            var durationDays = plan?.DurationInDays ?? 30;
            var now = DateTime.UtcNow;

            var activeSub = await _unitOfWork.UserSubscriptions.GetActiveSubscriptionByUserIdAsync(paymentTransaction.UserId, cancellationToken);
            if (activeSub != null)
            {
                activeSub.EndDate = activeSub.EndDate.AddDays(durationDays);
                activeSub.Status = SubscriptionStatus.Active;
                activeSub.UpdatedAt = now;
                _unitOfWork.UserSubscriptions.Update(activeSub);
                paymentTransaction.SubscriptionId = activeSub.Id;
            }
            else
            {
                var newSub = new UserSubscription
                {
                    UserId = paymentTransaction.UserId,
                    PlanId = paymentTransaction.PlanId,
                    Status = SubscriptionStatus.Active,
                    StartDate = now,
                    EndDate = now.AddDays(durationDays),
                    AutoRenew = false,
                    CreatedAt = now
                };
                await _unitOfWork.UserSubscriptions.AddAsync(newSub, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                paymentTransaction.SubscriptionId = newSub.Id;
            }

            _logger.LogInformation("Successfully activated subscription for User {UserId} until {EndDate}",
                paymentTransaction.UserId, paymentTransaction.Subscription?.EndDate);
        }
        else
        {
            paymentTransaction.Status = PaymentStatus.Failed;
            _logger.LogInformation("Payment transaction failed for Paymob Order ID {OrderId}", paymobOrderId);
        }

        _unitOfWork.PaymentTransactions.Update(paymentTransaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Subscriptions.WebhookProcessed);
    }

    public async Task<Result<IReadOnlyList<PaymentTransactionResponse>>> GetUserPaymentHistoryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var transactions = await _unitOfWork.PaymentTransactions.GetByUserIdAsync(userId, cancellationToken);
        var dtos = transactions.Select(t => new PaymentTransactionResponse
        {
            Id = t.Id,
            PaymobOrderId = t.PaymobOrderId,
            PaymobTransactionId = t.PaymobTransactionId,
            Amount = t.Amount,
            Currency = t.Currency,
            Status = t.Status,
            PaymentMethod = t.PaymentMethod,
            CreatedAt = t.CreatedAt
        }).ToList();

        return Result<IReadOnlyList<PaymentTransactionResponse>>.Ok(dtos, SuccessMessages.Subscriptions.GetPaymentHistory);
    }
}
