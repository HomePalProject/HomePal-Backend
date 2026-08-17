using System.ClientModel;
using System.Text;
using Google.GenAI;
using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Application.Features.MealPlanning.Interfaces;
using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Infrastructure.AI.CatalogManagement.Instructions;
using HomePal.Infrastructure.AI.CatalogManagement.Options;
using HomePal.Infrastructure.AI.CatalogManagement.Services;
using HomePal.Infrastructure.AI.MealPlanning.Instructions;
using HomePal.Infrastructure.AI.MealPlanning.Tools;
using HomePal.Infrastructure.AI.PantryManagement.Instructions;
using HomePal.Infrastructure.AI.PantryManagement.Options;
using HomePal.Infrastructure.AI.PantryManagement.Services;
using HomePal.Infrastructure.AI.PantryManagement.Tools;
using HomePal.Infrastructure.AI.Rag.Options;
using HomePal.Infrastructure.AI.Rag.Services;
using HomePal.Infrastructure.AI.Services;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenAI;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Microsoft.Agents.AI.Workflows;

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

        services.AddOptions<MongoRagOptions>()
            .Bind(configuration.GetSection(MongoRagOptions.SectionName));

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

        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoRagOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });

        services.AddAIAgent("PantryScannerAgent", instructions: PantryAgentInstructions.SystemInstructions);
        services.AddAIAgent("ProductScraperAgent", instructions: ProductScraperInstructions.SystemInstructions);

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

        services.AddScoped<HomePal.Infrastructure.AI.AgentChat.Services.AgentSseStream>();
        services.AddScoped<HomePal.Infrastructure.AI.Common.AgentUserContext>();
        services.AddSingleton<CalculatorTools>();
        services.AddScoped<MealPlanTools>();
        services.AddScoped<PantryTools>();
        services.AddScoped<BudgetTools>();
        services.AddScoped<ShoppingListTools>();
        services.AddScoped<CatalogReferenceTools>();
        services.AddScoped<HouseholdTools>();
        services.AddScoped<HomePal.Application.Features.AgentChat.Interfaces.IAgentChatService, HomePal.Infrastructure.AI.AgentChat.Services.AgentChatService>();

        // Specialist Sub-Agents
        services.AddKeyedScoped<AIAgent>("MealAndInventoryAgent", (sp, key) =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            var mealPlanTools = sp.GetRequiredService<MealPlanTools>();
            var pantryTools = sp.GetRequiredService<PantryTools>();
            var recipeTools = sp.GetRequiredService<RecipeSearchTools>();
            var catalogTools = sp.GetRequiredService<CatalogReferenceTools>();
            var householdTools = sp.GetRequiredService<HouseholdTools>();
            var budgetTools = sp.GetRequiredService<BudgetTools>();
            var offerTools = sp.GetRequiredService<OfferSearchTools>();
            var shoppingListTools = sp.GetRequiredService<ShoppingListTools>();
            var calcTools = sp.GetRequiredService<CalculatorTools>();
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                Id = "meal-and-inventory-agent",
                Name = "MealAndInventoryAgent",
                Description = "Handles household-aware recipes, pantry inventory, expiration tracking, meal plans, and budget evaluation for missing ingredients.",
                ChatOptions = new ChatOptions
                {
                    Instructions = MealAndInventoryInstructions.SystemInstructions,
                    Tools =
                    [
                        AIFunctionFactory.Create(householdTools.GetHouseholdMembersWithPreferencesAsync),
                        AIFunctionFactory.Create(pantryTools.GetPantryAsync),
                        AIFunctionFactory.Create(pantryTools.AddPantryItemAsync),
                        AIFunctionFactory.Create(pantryTools.UpdatePantryAsync),
                        AIFunctionFactory.Create(pantryTools.DeletePantryItemAsync),
                        AIFunctionFactory.Create(recipeTools.SearchRecipesAsync),
                        AIFunctionFactory.Create(budgetTools.GetCurrentBudgetAsync),
                        AIFunctionFactory.Create(offerTools.SearchOffersAsync),
                        AIFunctionFactory.Create(shoppingListTools.AddShoppingListItemAsync),
                        AIFunctionFactory.Create(mealPlanTools.SaveMealPlanAsync),
                        AIFunctionFactory.Create(mealPlanTools.GetLastMealPlanAsync),
                        AIFunctionFactory.Create(mealPlanTools.UpdateLastMealPlanAsync),
                        AIFunctionFactory.Create(catalogTools.GetCategoriesAndUnitsAsync),
                        AIFunctionFactory.Create(calcTools.Calculate)
                    ]
                }
            });
        });

        services.AddKeyedScoped<AIAgent>("NutritionAndHealthAgent", (sp, key) =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            var householdTools = sp.GetRequiredService<HouseholdTools>();
            var ingredientTools = sp.GetRequiredService<IngredientSearchTools>();
            var calcTools = sp.GetRequiredService<CalculatorTools>();
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                Id = "nutrition-and-health-agent",
                Name = "NutritionAndHealthAgent",
                Description = "Handles dietary guidelines, macronutrients, calorie targets, allergies, health restrictions, and ingredient nutrition.",
                ChatOptions = new ChatOptions
                {
                    Instructions = NutritionAndHealthInstructions.SystemInstructions,
                    Tools =
                    [
                        AIFunctionFactory.Create(householdTools.GetHouseholdMembersWithPreferencesAsync),
                        AIFunctionFactory.Create(ingredientTools.SearchIngredientsAsync),
                        AIFunctionFactory.Create(calcTools.Calculate)
                    ]
                }
            });
        });

        services.AddKeyedScoped<AIAgent>("BudgetAndShoppingAgent", (sp, key) =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            var shoppingListTools = sp.GetRequiredService<ShoppingListTools>();
            var budgetTools = sp.GetRequiredService<BudgetTools>();
            var offerTools = sp.GetRequiredService<OfferSearchTools>();
            var calcTools = sp.GetRequiredService<CalculatorTools>();
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                Id = "budget-and-shopping-agent",
                Name = "BudgetAndShoppingAgent",
                Description = "Handles household budget limits, shopping lists, grocery cost estimates, and supermarket offers.",
                ChatOptions = new ChatOptions
                {
                    Instructions = BudgetAndShoppingInstructions.SystemInstructions,
                    Tools =
                    [
                        AIFunctionFactory.Create(shoppingListTools.GetShoppingListAsync),
                        AIFunctionFactory.Create(shoppingListTools.AddShoppingListItemAsync),
                        AIFunctionFactory.Create(shoppingListTools.UpdateShoppingListItemAsync),
                        AIFunctionFactory.Create(shoppingListTools.DeleteShoppingListItemAsync),
                        AIFunctionFactory.Create(budgetTools.GetCurrentBudgetAsync),
                        AIFunctionFactory.Create(offerTools.SearchOffersAsync),
                        AIFunctionFactory.Create(calcTools.Calculate)
                    ]
                }
            });
        });


        services.AddKeyedScoped<AIAgent>("Agent", (sp, key) =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            var calcTools = sp.GetRequiredService<CalculatorTools>();

            var mealAndInventoryAgent = sp.GetRequiredKeyedService<AIAgent>("MealAndInventoryAgent");
            var nutritionAndHealthAgent = sp.GetRequiredKeyedService<AIAgent>("NutritionAndHealthAgent");
            var budgetAndShoppingAgent = sp.GetRequiredKeyedService<AIAgent>("BudgetAndShoppingAgent");

            var triageAgent = chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                Id = "triage-agent",
                Name = "TriageAgent",
                Description = "Primary entry point that routes user conversations to the appropriate specialist agent.",
                ChatOptions = new ChatOptions
                {
                    Instructions = TriageAgentInstructions.SystemInstructions,
                    Tools =
                    [
                        AIFunctionFactory.Create(calcTools.Calculate)
                    ]
                }
            });

            // Build Handoff Workflow
            var handoffBuilder = AgentWorkflowBuilder.CreateHandoffBuilderWith(triageAgent);

            // Triage -> Specialists
            handoffBuilder.WithHandoffs(
                triageAgent,
                [mealAndInventoryAgent, nutritionAndHealthAgent, budgetAndShoppingAgent]);

            // Specialists -> Triage
            handoffBuilder.WithHandoffs(
                [mealAndInventoryAgent, nutritionAndHealthAgent, budgetAndShoppingAgent],
                triageAgent);

            var workflow = handoffBuilder.Build();

            return workflow.AsAIAgent(
                id: "homepal-handoff",
                name: "HomePal Handoff Agent",
                description: "HomePal multi-agent handoff workflow.");
        });

        return services;
    }
}

