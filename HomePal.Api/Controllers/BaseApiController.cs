using System.Security.Claims;
using HomePal.Api.Resources;
using HomePal.Shared.Responses;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HomePal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private IStringLocalizer<SharedResource>? _localizer;
    protected IStringLocalizer<SharedResource> Localizer =>
        _localizer ??= HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();

    protected Guid CurrentUserId
    {
        get
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirstValue("sub");
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }

    protected IActionResult HandleResult(Result result)
    {
        return ProcessResult<object>(result, null);
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        return ProcessResult(result, result.Data);
    }

    private IActionResult ProcessResult<T>(Result result, T? data)
    {
        var localizedString = Localizer[result.Message];
        var message = !localizedString.ResourceNotFound 
            ? localizedString.Value 
            : result.Message;

        List<Error>? errors = null;
        if (!result.Success)
        {
            errors = new List<Error>();
            if (result.Errors != null && result.Errors.Count > 0)
            {
                foreach (var err in result.Errors)
                {
                    if (err is ValidationError valErr)
                    {
                        var loc = Localizer[valErr.Message];
                        var locMsg = !loc.ResourceNotFound ? loc.Value : valErr.Message;
                        errors.Add(new ValidationError(valErr.Field, locMsg));
                    }
                    else
                    {
                        var loc = Localizer[err.Message];
                        var locMsg = !loc.ResourceNotFound ? loc.Value : err.Message;
                        errors.Add(new Error(locMsg));
                    }
                }
            }
            else
            {
                errors.Add(new Error(message));
            }
        }

        var responsePayload = new ApiResponse<T>
        {
            Success = result.Success,
            Status = result.Status.ToString(),
            Message = message,
            Data = result.Success ? data : default,
            Errors = errors
        };

        return result.Status switch
        {
            ResultStatus.Success            => Ok(responsePayload),
            ResultStatus.Created            => StatusCode(StatusCodes.Status201Created, responsePayload),
            ResultStatus.NoContent          => NoContent(),
            ResultStatus.BadRequest         => BadRequest(responsePayload),
            ResultStatus.ValidationError    => BadRequest(responsePayload),
            ResultStatus.Unauthorized       => Unauthorized(responsePayload),
            ResultStatus.Forbidden          => StatusCode(StatusCodes.Status403Forbidden, responsePayload),
            ResultStatus.NotFound           => NotFound(responsePayload),
            ResultStatus.Conflict           => Conflict(responsePayload),
            ResultStatus.UnprocessableEntity=> StatusCode(StatusCodes.Status422UnprocessableEntity, responsePayload),
            ResultStatus.TooManyRequests    => StatusCode(StatusCodes.Status429TooManyRequests, responsePayload),
            ResultStatus.Failure            => StatusCode(StatusCodes.Status500InternalServerError, responsePayload),
            ResultStatus.ServiceUnavailable => StatusCode(StatusCodes.Status503ServiceUnavailable, responsePayload),
            _                               => BadRequest(responsePayload)
        };
    }
}
