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

        Dictionary<string, string>? errors = null;
        if (!result.Success)
        {
            errors = new Dictionary<string, string>();
            if (result.Errors != null && result.Errors.Count > 0)
            {
                foreach (var (field, key) in result.Errors)
                {
                    var loc = Localizer[key];
                    errors[field] = !loc.ResourceNotFound ? loc.Value : key;
                }
            }
            else
            {
                errors["general"] = message;
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
            ResultStatus.Success     => Ok(responsePayload),
            ResultStatus.Created     => Ok(responsePayload),
            ResultStatus.BadRequest  => BadRequest(responsePayload),
            ResultStatus.Unauthorized=> Unauthorized(responsePayload),
            ResultStatus.Forbidden   => StatusCode(StatusCodes.Status403Forbidden, responsePayload),
            ResultStatus.NotFound    => NotFound(responsePayload),
            ResultStatus.Conflict    => Conflict(responsePayload),
            ResultStatus.ValidationError => BadRequest(responsePayload),
            _                        => BadRequest(responsePayload)
        };
    }
}
