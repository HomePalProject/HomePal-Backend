using System.ClientModel;
using System.Text;
using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Infrastructure.AI.CatalogManagement.Instructions;
using HomePal.Infrastructure.AI.CatalogManagement.Options;
using HomePal.Infrastructure.AI.CatalogManagement.Services;
using HomePal.Infrastructure.AI.PantryManagement.Instructions;
using HomePal.Infrastructure.AI.PantryManagement.Options;
using HomePal.Infrastructure.AI.PantryManagement.Services;
using HomePal.Infrastructure.AI.Services;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HomePal.Api.Extensions;

public static class AIServicesExtensions
{
    public static IServiceCollection AddAIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AgentOptions>()
            .Bind(configuration.GetSection(AgentOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<ApifyOptions>()
            .Bind(configuration.GetSection(ApifyOptions.SectionName))
            .ValidateOnStart();

        var agentOptions = configuration.GetSection(AgentOptions.SectionName).Get<AgentOptions>() ?? new AgentOptions();
        if (agentOptions.Langfuse.Enabled)
        {
            var langfuse = agentOptions.Langfuse;
            var authBytes = Encoding.UTF8.GetBytes($"{langfuse.PublicKey}:{langfuse.SecretKey}");
            var base64Auth = Convert.ToBase64String(authBytes);

            var rawEndpoint = string.IsNullOrWhiteSpace(langfuse.Endpoint)
                ? "https://cloud.langfuse.com"
                : langfuse.Endpoint.TrimEnd('/');

            var endpoint = rawEndpoint.EndsWith("/api/public/otel/v1/traces", StringComparison.OrdinalIgnoreCase)
                ? rawEndpoint
                : $"{rawEndpoint}/api/public/otel/v1/traces";

            var serviceName = string.IsNullOrWhiteSpace(langfuse.ServiceName)
                ? "HomePal.AI"
                : langfuse.ServiceName;

            services.AddOpenTelemetry()
                .WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                        .AddSource("Experimental.Microsoft.Extensions.AI")
                        .AddSource("Microsoft.Extensions.AI")
                        .AddSource("Microsoft.Agents.AI")
                        .AddSource("OpenAI")
                        .AddSource("*")
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(endpoint);
                            options.Protocol = OtlpExportProtocol.HttpProtobuf;
                            options.Headers = $"Authorization=Basic {base64Auth},x-langfuse-ingestion-version=4";
                        });
                });
        }

        services.AddSingleton<IChatClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AgentOptions>>().Value;
            var modelId = string.IsNullOrWhiteSpace(options.ModelId) ? "gpt-4o" : options.ModelId;
            var apiKey = string.IsNullOrWhiteSpace(options.ApiKey) ? "mock-key" : options.ApiKey;
            var endpoint = options.Endpoint;

            OpenAIClientOptions? clientOptions = null;
            if (!string.IsNullOrWhiteSpace(endpoint) && Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri))
            {
                clientOptions = new OpenAIClientOptions
                {
                    Endpoint = endpointUri
                };
            }

            var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions);
            var openAiChatClient = openAiClient.GetChatClient(modelId);
            var baseChatClient = openAiChatClient.AsIChatClient();

            return baseChatClient
                .AsBuilder()
                .UseOpenTelemetry()
                .Build(sp);
        });

        services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AgentOptions>>().Value;
            var embeddingModelId = string.IsNullOrWhiteSpace(options.EmbeddingModelId) ? "gemini-embedding-001" : options.EmbeddingModelId;
            var apiKey = string.IsNullOrWhiteSpace(options.ApiKey) ? "mock-key" : options.ApiKey;
            var endpoint = options.Endpoint;

            OpenAIClientOptions? clientOptions = null;
            if (!string.IsNullOrWhiteSpace(endpoint) && Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri))
            {
                clientOptions = new OpenAIClientOptions
                {
                    Endpoint = endpointUri
                };
            }

            var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions);
            var embeddingClient = openAiClient.GetEmbeddingClient(embeddingModelId);
            return embeddingClient.AsIEmbeddingGenerator();
        });

        services.AddAIAgent("PantryScannerAgent", instructions: PantryAgentInstructions.SystemInstructions);
        services.AddAIAgent("ProductScraperAgent", instructions: ProductScraperInstructions.SystemInstructions);

        services.AddHttpClient<IApifyScraperService, ApifyScraperService>();
        services.AddSingleton<IScraperJobTracker, ScraperJobTracker>();
        services.AddScoped<IPantryScannerService, PantryAgentScanner>();
        services.AddScoped<IEmbeddingService, EmbeddingService>();
        services.AddScoped<IProductOfferScraperService, ProductOfferScraperAgent>();

        return services;
    }
}
