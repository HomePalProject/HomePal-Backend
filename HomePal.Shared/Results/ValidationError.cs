namespace HomePal.Shared.Results;

public class ValidationError : Error
{
    public string Field { get; set; } = string.Empty;

    public ValidationError() { }

    public ValidationError(string field, string message)
        : base(message)
    {
        Field = field;
    }
}
