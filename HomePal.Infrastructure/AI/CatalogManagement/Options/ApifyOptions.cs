namespace HomePal.Infrastructure.AI.CatalogManagement.Options;

public class ApifyOptions
{
    public const string SectionName = "ApifyOptions";

    public string ApiToken { get; set; } = string.Empty;
    public string ActorId { get; set; } = "apify~facebook-posts-scraper";
    public string BaseUrl { get; set; } = "https://api.apify.com/v2";
}
