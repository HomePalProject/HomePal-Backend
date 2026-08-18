using System.ClientModel;
using Google.GenAI;
using HomePal.Api.Extensions.OpenTelemetry;
using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.AgentChat.Interfaces;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Application.Features.MealPlanning.Interfaces;
using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Infrastructure.AI.AgentChat.Services;
using HomePal.Infrastructure.AI.CatalogManagement.Options;
using HomePal.Infrastructure.AI.CatalogManagement.Services;
using HomePal.Infrastructure.AI.Common;
using HomePal.Infrastructure.AI.MealPlanning.Tools;
using HomePal.Infrastructure.AI.PantryManagement.Options;
using HomePal.Infrastructure.AI.PantryManagement.Services;
using HomePal.Infrastructure.AI.PantryManagement.Tools;
using HomePal.Infrastructure.AI.Rag.Options;
using HomePal.Infrastructure.AI.Rag.Services;
using HomePal.Infrastructure.AI.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenAI;

namespace HomePal.Api.Extensions;

public static class AIServicesExtensions
{
    public static IServiceCollection AddAIServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Options Configuration
        services.AddOptions<AgentOptions>()
            .Bind(configuration.GetSection(AgentOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<ApifyOptions>()
            .Bind(configuration.GetSection(ApifyOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<MongoRagOptions>()
            .Bind(configuration.GetSection(MongoRagOptions.SectionName));

        // OpenTelemetry & Tracing Configuration
        services.AddAIOpenTelemetry(configuration);

        // Core AI Clients
        services.AddSingleton<IChatClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AgentOptions>>().Value;
            var provider = string.IsNullOrWhiteSpace(options.Provider) ? "Google" : options.Provider;

            var apiKey = string.IsNullOrWhiteSpace(options.ApiKey) || options.ApiKey == "API_KEY"
                ? Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "mock-key"
                : options.ApiKey;

            var modelId = string.IsNullOrWhiteSpace(options.ModelId) || options.ModelId == "MODEL_ID"
                ? Environment.GetEnvironmentVariable("GEMINI_MODEL") ?? "gemini-2.5-flash"
                : options.ModelId;

            IChatClient baseChatClient;

            if (provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
            {
                OpenAIClientOptions? clientOptions = null;
                if (!string.IsNullOrWhiteSpace(options.Endpoint) && Uri.TryCreate(options.Endpoint, UriKind.Absolute, out var endpointUri))
                {
                    clientOptions = new OpenAIClientOptions { Endpoint = endpointUri };
                }

                var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions);
                baseChatClient = openAiClient.GetChatClient(modelId).AsIChatClient();
            }
            else
            {
                // Default: Google GenAI (gemini-2.5-flash)
                var geminiClient = new Google.GenAI.Client(apiKey: apiKey);
                baseChatClient = geminiClient.AsIChatClient(modelId);
            }

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

        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoRagOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });

        // AI Background & Scanning Services
        services.AddHttpClient<IApifyScraperService, ApifyScraperService>();
        services.AddSingleton<IScraperJobTracker, ScraperJobTracker>();
        services.AddScoped<IPantryScannerService, PantryAgentScanner>();
        services.AddScoped<IEmbeddingService, EmbeddingService>();
        services.AddScoped<IProductOfferScraperService, ProductOfferScraperAgent>();

        // RAG / MongoDB Vector Search Services & Tools
        services.AddScoped<IFoodRecipeSearchService, FoodRecipeSearchService>();
        services.AddScoped<IIngredientSearchService, IngredientSearchService>();
        services.AddScoped<RecipeSearchTools>();
        services.AddScoped<IngredientSearchTools>();
        services.AddScoped<OfferSearchTools>();

        // Agent Chat Services & Tools
        services.AddScoped<AgentSseStream>();
        services.AddScoped<AgentUserContext>();
        services.AddSingleton<CalculatorTools>();
        services.AddScoped<MealPlanTools>();
        services.AddScoped<PantryTools>();
        services.AddScoped<BudgetTools>();
        services.AddScoped<ShoppingListTools>();
        services.AddScoped<CatalogReferenceTools>();
        services.AddScoped<HouseholdTools>();
        services.AddScoped<IAgentChatService, AgentChatService>();

        // AI Agents & Workflows
        services.AddAIAgents();

        return services;
    }
}
