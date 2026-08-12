using System.ComponentModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.DependencyInjection;

namespace HomePal.Infrastructure.AI.MealPlanning.Tools;

public class MealPlanningSubAgentTools
{
    private readonly AIAgent _mealAndInventoryAgent;
    private readonly AIAgent _nutritionAndHealthAgent;
    private readonly AIAgent _budgetAndShoppingAgent;

    public MealPlanningSubAgentTools(
        [FromKeyedServices("MealAndInventoryAgent")] AIAgent mealAndInventoryAgent,
        [FromKeyedServices("NutritionAndHealthAgent")] AIAgent nutritionAndHealthAgent,
        [FromKeyedServices("BudgetAndShoppingAgent")] AIAgent budgetAndShoppingAgent)
    {
        _mealAndInventoryAgent = mealAndInventoryAgent;
        _nutritionAndHealthAgent = nutritionAndHealthAgent;
        _budgetAndShoppingAgent = budgetAndShoppingAgent;
    }

    [Description("Consults the Meal & Inventory Specialist Agent to evaluate pantry items, expiration dates, available ingredients, and suggest recipes based on what's currently in stock.")]
    public async Task<string> ConsultMealAndInventoryAsync(
        [Description("The specific query regarding pantry stock, expiring items, or ingredient-based recipe ideas.")] string query,
        CancellationToken cancellationToken = default)
    {
        var response = await _mealAndInventoryAgent.RunAsync(query, cancellationToken: cancellationToken);
        return response.Text;
    }

    [Description("Consults the Nutrition & Health Specialist Agent to analyze dietary guidelines, macronutrients, calorie targets, allergies, and health restrictions.")]
    public async Task<string> ConsultNutritionAndHealthAsync(
        [Description("The specific query regarding nutritional requirements, health goals, dietary restrictions, or calorie breakdowns.")] string query,
        CancellationToken cancellationToken = default)
    {
        var response = await _nutritionAndHealthAgent.RunAsync(query, cancellationToken: cancellationToken);
        return response.Text;
    }

    [Description("Consults the Budget & Shopping Specialist Agent to check household budget constraints, estimate meal preparation costs, and look up supermarket deals or missing items.")]
    public async Task<string> ConsultBudgetAndShoppingAsync(
        [Description("The query regarding budget limits, ingredient pricing, cost estimations, or shopping list items.")] string query,
        CancellationToken cancellationToken = default)
    {
        var response = await _budgetAndShoppingAgent.RunAsync(query, cancellationToken: cancellationToken);
        return response.Text;
    }
}
