namespace HomePal.Shared.Results;

public enum ResultStatus
{
    // Success (2xx)
    Success,
    Created,
    Accepted,
    NoContent,

    // Client Errors (4xx)
    BadRequest,
    ValidationError,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    UnprocessableEntity,
    TooManyRequests,

    // Server Errors (5xx)
    Failure,
    ServiceUnavailable
}
