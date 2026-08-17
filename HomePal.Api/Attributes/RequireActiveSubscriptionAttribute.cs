using System.Security.Claims;
using HomePal.Api.Resources;
using HomePal.Application.Features.Subscriptions.Interfaces;
using HomePal.Application.Features.Subscriptions.Options;
using HomePal.Shared.Responses;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace HomePal.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class RequireActiveSubscriptionAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 1. Check if subscription enforcement is disabled via configuration
        var subscriptionOptions = context.HttpContext.RequestServices
            .GetService<IOptions<SubscriptionOptions>>()?.Value;

        if (subscriptionOptions != null && !subscriptionOptions.Enabled)
        {
            await next();
            return;
        }

        var localizer = context.HttpContext.RequestServices.GetService<IStringLocalizer<SharedResource>>();
        var user = context.HttpContext.User;
        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            var msg = localizer?[ErrorMessages.Auth.AccountInactive] ?? "Authentication required.";
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                Success = false,
                Status = ResultStatus.Unauthorized.ToString(),
                Message = msg,
                Errors = new List<Error> { new(msg) }
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? user.FindFirst("sub")?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId) || userId == Guid.Empty)
        {
            var msg = localizer?[ErrorMessages.Auth.UserNotFound] ?? "Invalid user identification.";
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                Success = false,
                Status = ResultStatus.Unauthorized.ToString(),
                Message = msg,
                Errors = new List<Error> { new(msg) }
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        var subscriptionService = context.HttpContext.RequestServices.GetRequiredService<ISubscriptionService>();
        var hasActiveSub = await subscriptionService.HasActiveSubscriptionAsync(userId, context.HttpContext.RequestAborted);

        if (!hasActiveSub)
        {
            var msg = localizer?[ErrorMessages.Subscriptions.ActiveSubscriptionRequired] ?? "An active subscription is required to access this feature.";
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                Success = false,
                Status = ResultStatus.Forbidden.ToString(),
                Message = msg,
                Errors = new List<Error> { new(msg) }
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }

        await next();
    }
}
