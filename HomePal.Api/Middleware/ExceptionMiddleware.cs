using System.Net;
using System.Text.Json;
using HomePal.Api.Resources;
using HomePal.Shared.Responses;
using HomePal.Shared.Results;
using Microsoft.Extensions.Localization;

namespace HomePal.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var localizer = context.RequestServices.GetService<IStringLocalizer<SharedResource>>();
        var localizedMessage = localizer?[ErrorMessages.Server.InternalError].Value ?? "An unexpected server error occurred.";

        var message = _env.IsDevelopment() ? exception.Message : localizedMessage;
        var errorsDict = new Dictionary<string, string>
        {
            { "general", message }
        };

        var response = ApiResponse.FailureResponse(message, ResultStatus.Failure.ToString(), errorsDict);
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
