using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Infrastructure.AI.CatalogManagement.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HomePal.Infrastructure.AI.CatalogManagement.Services;

public class ApifyScraperService : IApifyScraperService
{
    private readonly HttpClient _httpClient;
    private readonly ApifyOptions _apifyOptions;
    private readonly ILogger<ApifyScraperService> _logger;

    public ApifyScraperService(
        HttpClient httpClient,
        IOptions<ApifyOptions> apifyOptions,
        ILogger<ApifyScraperService> logger)
    {
        _httpClient = httpClient;
        _apifyOptions = apifyOptions.Value;
        _logger = logger;
    }

    public async Task<List<FacebookPostDto>> FetchFacebookPostsAsync(
        string pageUrl,
        int daysBack = 5,
        int resultsLimit = 20,
        CancellationToken cancellationToken = default)
    {
        var token = _apifyOptions.ApiToken;
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Apify API Token is missing in configuration.");
            return new List<FacebookPostDto>();
        }

        var actorId = !string.IsNullOrWhiteSpace(_apifyOptions.ActorId) ? _apifyOptions.ActorId : "apify~facebook-posts-scraper";
        var baseUrl = !string.IsNullOrWhiteSpace(_apifyOptions.BaseUrl) ? _apifyOptions.BaseUrl : "https://api.apify.com/v2";

        var input = new
        {
            startUrls = new[] { new { url = pageUrl } },
            onlyPostsNewerThan = DateTime.Today.AddDays(-daysBack).ToString("yyyy-MM-dd"),
            resultsLimit = resultsLimit > 0 ? resultsLimit : 20
        };

        var endpoint = $"{baseUrl.TrimEnd('/')}/acts/{actorId}/run-sync-get-dataset-items?token={token}";

        try
        {
            var content = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var readOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var posts = await response.Content.ReadFromJsonAsync<List<ApifyRawPost>>(readOptions, cancellationToken);

            if (posts == null || posts.Count == 0)
                return new List<FacebookPostDto>();

            var result = posts.Select((p, index) => new FacebookPostDto
            {
                Id = index,
                Text = p.Text,
                PostUrl = p.PostUrl ?? p.Url ?? p.FacebookUrl,
                Media = p.Media?.Select(m => new FacebookMediaDto
                {
                    ImgUrl = m.Photo_Image?.Uri ?? m.Image?.Uri,
                    OcrText = m.OcrText
                }).Where(m => !string.IsNullOrWhiteSpace(m.ImgUrl)).ToList()
            }).ToList();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Facebook posts from Apify API for page {PageUrl}", pageUrl);
            return new List<FacebookPostDto>();
        }
    }

    private class ApifyRawPost
    {
        public string? Text { get; set; }
        public string? Url { get; set; }
        public string? PostUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public List<ApifyRawMedia>? Media { get; set; }
    }

    private class ApifyRawMedia
    {
        public ApifyRawImage? Photo_Image { get; set; }
        public ApifyRawImage? Image { get; set; }
        public string? OcrText { get; set; }
    }

    private class ApifyRawImage
    {
        public string? Uri { get; set; }
    }
}
