namespace HomePal.Infrastructure.Google;

public class GoogleAuthOptions
{
    public const string SectionName = "GoogleAuthOptions";

    public string ClientId { get; set; } = string.Empty;

    public string AndroidClientId { get; set; } = string.Empty;
}

