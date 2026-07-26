using System.Text.Json.Serialization;

namespace HomePal.Shared.Results;

[JsonDerivedType(typeof(ValidationError))]
public class Error
{
    public string Message { get; set; } = string.Empty;

    public Error() { }

    public Error(string message)
    {
        Message = message;
    }
}
