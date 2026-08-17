using HomePal.Application.Features.Subscriptions.DTOs;
using HomePal.Application.Features.Subscriptions.Interfaces;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Route("api/subscriptions")]
public class SubscriptionsController : BaseApiController
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    /// <summary>
    /// Gets all active subscription plans.
    /// </summary>
    [HttpGet("plans")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SubscriptionPlanResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans(CancellationToken cancellationToken)
    {
        var result = await _subscriptionService.GetPlansAsync(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Gets current authenticated user's subscription details and status.
    /// </summary>
    [HttpGet("current")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserSubscriptionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentSubscription(CancellationToken cancellationToken)
    {
        var result = await _subscriptionService.GetCurrentUserSubscriptionAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Initiates a Paymob payment for a subscription plan and returns checkout URL.
    /// </summary>
    [HttpPost("checkout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<InitiatePaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Checkout([FromBody] InitiatePaymentRequest request, CancellationToken cancellationToken)
    {
        var result = await _subscriptionService.InitiatePaymentAsync(CurrentUserId, request.PlanId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Gets payment and transaction history for the authenticated user.
    /// </summary>
    [HttpGet("history")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PaymentTransactionResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPaymentHistory(CancellationToken cancellationToken)
    {
        var result = await _subscriptionService.GetUserPaymentHistoryAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Webhook endpoint called by Paymob to update payment and subscription status.
    /// </summary>
    [HttpPost("paymob-webhook")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PaymobWebhook(
        [FromBody] PaymobWebhookPayload payload,
        [FromQuery] string? hmac,
        CancellationToken cancellationToken)
    {
        // Paymob can supply HMAC via query param or inside payload
        if (string.IsNullOrWhiteSpace(payload.Hmac) && !string.IsNullOrWhiteSpace(hmac))
        {
            payload.Hmac = hmac;
        }

        var result = await _subscriptionService.ProcessPaymobWebhookAsync(payload, cancellationToken);
        return HandleResult(result);
    }
}
