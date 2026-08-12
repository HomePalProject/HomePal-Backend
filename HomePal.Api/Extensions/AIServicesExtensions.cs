using System.ClientModel;
using System.Text;
using Google.GenAI;
using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Infrastructure.AI.CatalogManagement.Instructions;
using HomePal.Infrastructure.AI.CatalogManagement.Options;
using HomePal.Infrastructure.AI.CatalogManagement.Services;
using HomePal.Infrastructure.AI.MealPlanning.Instructions;
using HomePal.Infrastructure.AI.MealPlanning.Tools;
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

        services.AddAIAgent("PantryScannerAgent", instructions: PantryAgentInstructions.SystemInstructions);
        services.AddAIAgent("ProductScraperAgent", instructions: ProductScraperInstructions.SystemInstructions);

        services.AddHttpClient<IApifyScraperService, ApifyScraperService>();
        services.AddSingleton<IScraperJobTracker, ScraperJobTracker>();
        services.AddScoped<IPantryScannerService, PantryAgentScanner>();
        services.AddScoped<IEmbeddingService, EmbeddingService>();
        services.AddScoped<IProductOfferScraperService, ProductOfferScraperAgent>();

        services.AddScoped<ChatHistoryProvider, HomePal.Infrastructure.AI.AgentChat.Services.DbChatHistoryProvider>();
        services.AddScoped<HomePal.Infrastructure.AI.AgentChat.Services.AgentSseStream>();
        services.AddSingleton<HomePal.Infrastructure.AI.AgentChat.Tools.CalculatorTools>();
        services.AddScoped<MealPlanningSubAgentTools>();
        services.AddScoped<HomePal.Application.Features.AgentChat.Interfaces.IAgentChatService, HomePal.Infrastructure.AI.AgentChat.Services.AgentChatService>();

        // Specialist Sub-Agents
        services.AddKeyedScoped<AIAgent>("MealAndInventoryAgent", (sp, key) =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                Name = "MealAndInventoryAgent",
                ChatOptions = new ChatOptions
                {
                    Instructions = MealAndInventoryInstructions.SystemInstructions
                }
            });
        });

        services.AddKeyedScoped<AIAgent>("NutritionAndHealthAgent", (sp, key) =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                Name = "NutritionAndHealthAgent",
                ChatOptions = new ChatOptions
                {
                    Instructions = NutritionAndHealthInstructions.SystemInstructions
                }
            });
        });

        services.AddKeyedScoped<AIAgent>("BudgetAndShoppingAgent", (sp, key) =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                Name = "BudgetAndShoppingAgent",
                ChatOptions = new ChatOptions
                {
                    Instructions = BudgetAndShoppingInstructions.SystemInstructions
                }
            });
        });

        // Master Meal Planning Supervisor Agent (Registered as "Agent")
        services.AddKeyedScoped<AIAgent>("Agent", (sp, key) =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            var historyProvider = sp.GetRequiredService<ChatHistoryProvider>();
            var calcTools = sp.GetRequiredService<HomePal.Infrastructure.AI.AgentChat.Tools.CalculatorTools>();
            var subAgentTools = sp.GetRequiredService<MealPlanningSubAgentTools>();

            var options = new ChatClientAgentOptions
            {
                Name = "MealPlanningSupervisorAgent",
                ChatOptions = new ChatOptions
                {
                    Instructions = MealPlanningSupervisorInstructions.SystemInstructions,
                    Tools =
                    [
                        new ApprovalRequiredAIFunction(AIFunctionFactory.Create(calcTools.Calculate)),
                        AIFunctionFactory.Create(subAgentTools.ConsultMealAndInventoryAsync),
                        AIFunctionFactory.Create(subAgentTools.ConsultNutritionAndHealthAsync),
                        AIFunctionFactory.Create(subAgentTools.ConsultBudgetAndShoppingAsync)
                    ]
                },
                ChatHistoryProvider = historyProvider
            };

            return chatClient.AsAIAgent(options);
        });

        return services;
    }
}

