using HomePal.Application.Features.Subscriptions.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Subscriptions.Interfaces;

public interface ISubscriptionService
{
    Task<Result<IReadOnlyList<SubscriptionPlanResponse>>> GetPlansAsync(CancellationToken cancellationToken = default);

    Task<Result<UserSubscriptionResponse>> GetCurrentUserSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> HasActiveSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<InitiatePaymentResponse>> InitiatePaymentAsync(Guid userId, Guid planId, CancellationToken cancellationToken = default);

    Task<Result> ProcessPaymobWebhookAsync(PaymobWebhookPayload payload, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<PaymentTransactionResponse>>> GetUserPaymentHistoryAsync(Guid userId, CancellationToken cancellationToken = default);
}
