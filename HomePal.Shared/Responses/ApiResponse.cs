using HomePal.Shared.Results;

namespace HomePal.Shared.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<Error>? Errors { get; set; }

    public static ApiResponse<T> SuccessResponse(T? data, string message, string status = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Status = status,
            Message = message,
            Data = data,
            Errors = null
        };
    }

    public static ApiResponse<T> FailureResponse(string message, string status = "BadRequest", List<Error>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Status = status,
            Message = message,
            Data = default,
            Errors = errors ?? new List<Error> { new Error(message) }
        };
    }
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse SuccessResponse(string message, string status = "Success")
    {
        return new ApiResponse
        {
            Success = true,
            Status = status,
            Message = message,
            Data = null,
            Errors = null
        };
    }

    public static new ApiResponse FailureResponse(string message, string status = "BadRequest", List<Error>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Status = status,
            Message = message,
            Data = null,
            Errors = errors ?? new List<Error> { new Error(message) }
        };
    }
}
