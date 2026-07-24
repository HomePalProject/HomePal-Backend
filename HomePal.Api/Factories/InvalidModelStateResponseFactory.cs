using System.Text.Json;
using HomePal.Api.Resources;
using HomePal.Shared.Responses;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HomePal.Api.Factories;

public static class InvalidModelStateResponseFactory
{
    public static IActionResult ProduceResponse(ActionContext context)
    {
        var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();

        var errorsDict = new Dictionary<string, string>();

        foreach (var state in context.ModelState.Where(x => x.Value?.Errors.Count > 0))
        {
            var fieldName = JsonNamingPolicy.CamelCase.ConvertName(state.Key);
            var firstError = state.Value!.Errors.First();
            var errorKey = !string.IsNullOrEmpty(firstError.ErrorMessage)
                ? firstError.ErrorMessage
                : firstError.Exception?.Message ?? ErrorMessages.Validation.General;

            var localizedString = localizer[errorKey];
            var localizedMessage = !localizedString.ResourceNotFound ? localizedString.Value : errorKey;

            errorsDict[fieldName] = localizedMessage;
        }

        var messageLocalized = localizer[ErrorMessages.Validation.General].Value;
        var response = ApiResponse.FailureResponse(messageLocalized, ResultStatus.ValidationError.ToString(), errorsDict);
        return new BadRequestObjectResult(response);
    }
}
