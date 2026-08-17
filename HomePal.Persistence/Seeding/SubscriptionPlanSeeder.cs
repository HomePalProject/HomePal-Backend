using HomePal.Domain.Common;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HomePal.Persistence.Seeding;

public static class SubscriptionPlanSeeder
{
    public static async Task SeedSubscriptionPlansAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!await context.SubscriptionPlans.AnyAsync(p => p.Code == "monthly_chatbot"))
        {
            var monthlyPlan = new SubscriptionPlan
            {
                Code = "monthly_chatbot",
                Name = new List<LocalizedItem>
                {
                    new("en", "Monthly AI Chatbot Plan"),
                    new("ar", "الباقة الشهرية للمساعد الذكي")
                },
                Description = new List<LocalizedItem>
                {
                    new("en", "Unlimited access to HomePal AI Assistant, meal planning, and smart recipe generation for 30 days."),
                    new("ar", "وصول غير محدود للمساعد الذكي وتخطيط الوجبات والوصفات الذكية لمدة 30 يوم.")
                },
                Price = 150m,
                Currency = "EGP",
                DurationInDays = 30,
                IsActive = true
            };

            await context.SubscriptionPlans.AddAsync(monthlyPlan);
        }

        if (!await context.SubscriptionPlans.AnyAsync(p => p.Code == "annual_chatbot"))
        {
            var annualPlan = new SubscriptionPlan
            {
                Code = "annual_chatbot",
                Name = new List<LocalizedItem>
                {
                    new("en", "Annual AI Chatbot Plan"),
                    new("ar", "الباقة السنوية للمساعد الذكي")
                },
                Description = new List<LocalizedItem>
                {
                    new("en", "Unlimited access to HomePal AI Assistant for a full year with special discount."),
                    new("ar", "وصول غير محدود للمساعد الذكي لمدة عام كامل مع خصم خاص.")
                },
                Price = 1200m,
                Currency = "EGP",
                DurationInDays = 365,
                IsActive = true
            };

            await context.SubscriptionPlans.AddAsync(annualPlan);
        }

        await context.SaveChangesAsync();
    }
}
