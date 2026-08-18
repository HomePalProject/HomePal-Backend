using HomePal.Infrastructure.AI.CatalogManagement.Instructions;
using HomePal.Infrastructure.AI.MealPlanning.Instructions;
using HomePal.Infrastructure.AI.MealPlanning.Tools;
using HomePal.Infrastructure.AI.PantryManagement.Instructions;
using HomePal.Infrastructure.AI.PantryManagement.Tools;
using HomePal.Infrastructure.AI.Services;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace HomePal.Api.Extensions;

public static class AIAgentsExtensions
{
    public static IServiceCollection AddAIAgents(this IServiceCollection services)
    {
        // Standalone Agents
        services.AddAIAgent("PantryScannerAgent", instructions: PantryAgentInstructions.SystemInstructions);
        services.AddAIAgent("ProductScraperAgent", instructions: ProductScraperInstructions.SystemInstructions);

        // Specialist Sub-Agents
        services.AddMealAndInventoryAgent();
        services.AddNutritionAndHealthAgent();
        services.AddBudgetAndShoppingAgent();

        // Main Multi-Agent Handoff Workflow
        services.AddMainAgentWorkflow();

        return services;
    }

    private static IServiceCollection AddMealAndInventoryAgent(this IServiceCollection services)
    {
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

        return services;
    }

    private static IServiceCollection AddNutritionAndHealthAgent(this IServiceCollection services)
    {
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

        return services;
    }

    private static IServiceCollection AddBudgetAndShoppingAgent(this IServiceCollection services)
    {
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

        return services;
    }

    private static IServiceCollection AddMainAgentWorkflow(this IServiceCollection services)
    {
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
