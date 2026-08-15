using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HomePal.Application.Features.Reports.DTOs;
using HomePal.Application.Features.Reports.Interfaces;
using HomePal.Infrastructure.AI.PantryManagement.Options;
using HomePal.Shared.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HomePal.Infrastructure.AI.Services;

public class LangfuseMetricsService : ILangfuseMetricsService
{
    private readonly HttpClient _httpClient;
    private readonly AgentOptions _options;
    private readonly ILogger<LangfuseMetricsService> _logger;

    public LangfuseMetricsService(
        HttpClient httpClient,
        IOptions<AgentOptions> options,
        ILogger<LangfuseMetricsService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Result<TokenUsageMetricsDto>> GetTokenMetricsAsync(
        TokenMetricsFilterDto? filter = null,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var from = filter?.FromTimestamp ?? new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = filter?.ToTimestamp ?? now;

        var resultDto = new TokenUsageMetricsDto
        {
            FromTimestamp = from,
            ToTimestamp = to
        };

        var langfuse = _options.Langfuse;
        if (!langfuse.Enabled || string.IsNullOrWhiteSpace(langfuse.PublicKey) || string.IsNullOrWhiteSpace(langfuse.SecretKey))
        {
            _logger.LogInformation("Langfuse is disabled or keys are missing; returning default token metrics.");
            return Result<TokenUsageMetricsDto>.Ok(resultDto, "Analytics.FetchSuccess");
        }

        try
        {
            var rawEndpoint = string.IsNullOrWhiteSpace(langfuse.Endpoint)
                ? "https://us.cloud.langfuse.com"
                : langfuse.Endpoint.TrimEnd('/');

            string baseUrl;
            if (Uri.TryCreate(rawEndpoint, UriKind.Absolute, out var uri))
            {
                baseUrl = $"{uri.Scheme}://{uri.Authority}";
            }
            else
            {
                baseUrl = "https://us.cloud.langfuse.com";
            }

            var requestUrl = $"{baseUrl}/api/public/v2/metrics";

            var queryObj = new
            {
                view = "observations",
                metrics = new object[]
                {
                    new { measure = "inputTokens", aggregation = "sum" },
                    new { measure = "outputTokens", aggregation = "sum" },
                    new { measure = "totalTokens", aggregation = "sum" },
                    new { measure = "totalCost", aggregation = "sum" }
                },
                dimensions = Array.Empty<string>(),
                filters = Array.Empty<object>(),
                fromTimestamp = from.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                toTimestamp = to.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            var queryJson = JsonSerializer.Serialize(queryObj);
            var fullUrl = $"{requestUrl}?query={Uri.EscapeDataString(queryJson)}";

            using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            var authBytes = Encoding.UTF8.GetBytes($"{langfuse.PublicKey}:{langfuse.SecretKey}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Langfuse metrics API responded with status {StatusCode}: {Error}", response.StatusCode, errorContent);
                return Result<TokenUsageMetricsDto>.Ok(resultDto, "Analytics.FetchSuccess");
            }

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var parsed = JsonSerializer.Deserialize<LangfuseResponse>(responseBody);

            if (parsed?.Data != null)
            {
                foreach (var item in parsed.Data)
                {
                    resultDto.InputTokens += ParseLong(item.SumInputTokens);
                    resultDto.OutputTokens += ParseLong(item.SumOutputTokens);
                    resultDto.TotalTokens += ParseLong(item.SumTotalTokens);
                    resultDto.TotalCost += ParseDecimal(item.SumTotalCost);
                }
            }

            return Result<TokenUsageMetricsDto>.Ok(resultDto, "Analytics.FetchSuccess");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch metrics from Langfuse API");
            return Result<TokenUsageMetricsDto>.Ok(resultDto, "Analytics.FetchSuccess");
        }
    }

    private static long ParseLong(JsonElement el)
    {
        if (el.ValueKind == JsonValueKind.Number && el.TryGetInt64(out var n)) return n;
        if (el.ValueKind == JsonValueKind.String && long.TryParse(el.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var s)) return s;
        return 0;
    }

    private static decimal ParseDecimal(JsonElement el)
    {
        if (el.ValueKind == JsonValueKind.Number && el.TryGetDecimal(out var n)) return n;
        if (el.ValueKind == JsonValueKind.String && decimal.TryParse(el.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var s)) return s;
        return 0m;
    }

    private sealed class LangfuseResponse
    {
        [JsonPropertyName("data")]
        public List<LangfuseMetricData>? Data { get; set; }
    }

    private sealed class LangfuseMetricData
    {
        [JsonPropertyName("sum_inputTokens")]
        public JsonElement SumInputTokens { get; set; }

        [JsonPropertyName("sum_outputTokens")]
        public JsonElement SumOutputTokens { get; set; }

        [JsonPropertyName("sum_totalTokens")]
        public JsonElement SumTotalTokens { get; set; }

        [JsonPropertyName("sum_totalCost")]
        public JsonElement SumTotalCost { get; set; }
    }
}
