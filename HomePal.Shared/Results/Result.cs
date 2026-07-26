namespace HomePal.Shared.Results;

public class Result
{
    public bool Success { get; }
    public string Message { get; set; } = string.Empty;
    public List<Error>? Errors { get; set; }
    public ResultStatus Status { get; }

    protected Result(bool success, ResultStatus status, string message = "", List<Error>? errors = null)
    {
        Success = success;
        Status = status;
        Message = message;
        Errors = errors;
    }

    public static Result Ok(string messageKey = SuccessMessages.General, ResultStatus status = ResultStatus.Success)
        => new(true, status, messageKey);

    public static Result Fail(string messageKey = ErrorMessages.General, ResultStatus status = ResultStatus.BadRequest, List<Error>? errors = null)
        => new(false, status, messageKey, errors);
}

public class Result<T> : Result
{
    public T? Data { get; }

    internal Result(bool success, T? data, ResultStatus status, string message = "", List<Error>? errors = null)
        : base(success, status, message, errors)
    {
        Data = data;
    }

    public static Result<T> Ok(T data, string messageKey = SuccessMessages.General, ResultStatus status = ResultStatus.Success)
        => new(true, data, status, messageKey);

    public static new Result<T> Fail(string messageKey = ErrorMessages.General, ResultStatus status = ResultStatus.BadRequest, List<Error>? errors = null)
        => new(false, default, status, messageKey, errors);
}
