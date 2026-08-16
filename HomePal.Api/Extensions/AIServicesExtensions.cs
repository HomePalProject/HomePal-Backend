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

        services.AddScoped<ChatHistoryProvider, HomePal.Infrastructure.AI.AgentChat.Services.DbChatHistoryProvider>();
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
            var pantryTools = sp.GetRequiredService<PantryTools>();
            var catalogTools = sp.GetRequiredService<CatalogReferenceTools>();
            var recipeTools = sp.GetRequiredService<RecipeSearchTools>();
            var ingredientTools = sp.GetRequiredService<IngredientSearchTools>();
            var offerTools = sp.GetRequiredService<OfferSearchTools>();
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                Name = "MealAndInventoryAgent",
                Description = "Evaluates pantry stock, expiration dates, available ingredients, manages pantry inventory items (viewing, adding, updating, and removing stock), and searches real recipes and ingredients.",
                ChatOptions = new ChatOptions
                {
                    Instructions = MealAndInventoryInstructions.SystemInstructions,
                    Tools =
                    [
                        AIFunctionFactory.Create(pantryTools.GetPantryAsync),
                        AIFunctionFactory.Create(pantryTools.AddPantryItemAsync),
                        AIFunctionFactory.Create(pantryTools.UpdatePantryAsync),
                        AIFunctionFactory.Create(pantryTools.DeletePantryItemAsync),
                        AIFunctionFactory.Create(catalogTools.GetCategoriesAndUnitsAsync),
                        AIFunctionFactory.Create(recipeTools.SearchRecipesAsync),
                        AIFunctionFactory.Create(ingredientTools.SearchIngredientsAsync),
                        AIFunctionFactory.Create(offerTools.SearchOffersAsync)
                    ]
                }
            });
        });

        services.AddKeyedScoped<AIAgent>("NutritionAndHealthAgent", (sp, key) =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            var householdTools = sp.GetRequiredService<HouseholdTools>();
            var ingredientTools = sp.GetRequiredService<IngredientSearchTools>();
            var recipeTools = sp.GetRequiredService<RecipeSearchTools>();
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                Name = "NutritionAndHealthAgent",
                Description = "Analyzes dietary guidelines, macronutrients, calorie targets, allergies, health restrictions, and searches ingredients and recipes for nutritional details.",
                ChatOptions = new ChatOptions
                {
                    Instructions = NutritionAndHealthInstructions.SystemInstructions,
                    Tools =
                    [
                        AIFunctionFactory.Create(householdTools.GetHouseholdMembersWithPreferencesAsync),
                        AIFunctionFactory.Create(ingredientTools.SearchIngredientsAsync),
                        AIFunctionFactory.Create(recipeTools.SearchRecipesAsync)
                    ]
                }
            });
        });

        services.AddKeyedScoped<AIAgent>("BudgetAndShoppingAgent", (sp, key) =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            var calcTools = sp.GetRequiredService<CalculatorTools>();
            var budgetTools = sp.GetRequiredService<BudgetTools>();
            var shoppingListTools = sp.GetRequiredService<ShoppingListTools>();
            var catalogTools = sp.GetRequiredService<CatalogReferenceTools>();
            var offerTools = sp.GetRequiredService<OfferSearchTools>();
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                Name = "BudgetAndShoppingAgent",
                Description = "Checks household budget constraints, current monthly budget, remaining balance, estimates meal preparation costs, and manages shopping lists (adding, updating, viewing, and removing shopping list items).",
                ChatOptions = new ChatOptions
                {
                    Instructions = BudgetAndShoppingInstructions.SystemInstructions,
                    Tools =
                    [
                        AIFunctionFactory.Create(calcTools.Calculate),
                        AIFunctionFactory.Create(budgetTools.GetCurrentBudgetAsync),
                        AIFunctionFactory.Create(shoppingListTools.GetShoppingListAsync),
                        AIFunctionFactory.Create(shoppingListTools.AddShoppingListItemAsync),
                        AIFunctionFactory.Create(shoppingListTools.UpdateShoppingListItemAsync),
                        AIFunctionFactory.Create(shoppingListTools.DeleteShoppingListItemAsync),
                        AIFunctionFactory.Create(catalogTools.GetCategoriesAndUnitsAsync),
                        AIFunctionFactory.Create(offerTools.SearchOffersAsync)
                    ]
                }
            });
        });

        // Master Meal Planning Supervisor Agent (Registered as "Agent")
        services.AddKeyedScoped<AIAgent>("Agent", (sp, key) =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            var historyProvider = sp.GetRequiredService<ChatHistoryProvider>();
            var mealPlanTools = sp.GetRequiredService<MealPlanTools>();

            var mealAndInventoryAgent = sp.GetRequiredKeyedService<AIAgent>("MealAndInventoryAgent");
            var nutritionAndHealthAgent = sp.GetRequiredKeyedService<AIAgent>("NutritionAndHealthAgent");
            var budgetAndShoppingAgent = sp.GetRequiredKeyedService<AIAgent>("BudgetAndShoppingAgent");

            var options = new ChatClientAgentOptions
            {
                Name = "MealPlanningSupervisorAgent",
                ChatOptions = new ChatOptions
                {
                    Instructions = MealPlanningSupervisorInstructions.SystemInstructions,
                    Tools =
                    [
                        mealAndInventoryAgent.AsAIFunction(),
                        nutritionAndHealthAgent.AsAIFunction(),
                        budgetAndShoppingAgent.AsAIFunction(),
                        AIFunctionFactory.Create(mealPlanTools.SaveMealPlanAsync),
                        AIFunctionFactory.Create(mealPlanTools.GetLastMealPlanAsync),
                        AIFunctionFactory.Create(mealPlanTools.UpdateLastMealPlanAsync)
                    ]
                },
                ChatHistoryProvider = historyProvider
            };

            return chatClient.AsAIAgent(options);
        });

        return services;
    }
}

