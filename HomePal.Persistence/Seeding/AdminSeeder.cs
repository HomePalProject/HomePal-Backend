using HomePal.Application.Features.Auth.Options;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;
using HomePal.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomePal.Persistence.Seeding;

public static class AdminSeeder
{
    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AdminSeeder");

        var adminOptions = configuration.GetSection(AdminOptions.SectionName).Get<AdminOptions>() ?? new AdminOptions();

        if (!await roleManager.RoleExistsAsync(Roles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Admin));
        }

        var existingAdmin = await userManager.FindByEmailAsync(adminOptions.Email)
                            ?? await userManager.FindByNameAsync(adminOptions.Username);

        if (existingAdmin == null)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<HomePal.Persistence.Context.ApplicationDbContext>();
            var defaultGov = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(dbContext.Governorates, g => g.Code == "CAI");
            var defaultCity = defaultGov != null 
                ? await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(dbContext.Cities, c => c.GovernorateId == defaultGov.Id)
                : null;

            var adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = adminOptions.FullName,
                UserName = adminOptions.Username,
                Email = adminOptions.Email,
                EmailConfirmed = true,
                IsActive = true,
                IsProfileComplete = true,
                GovernorateId = defaultGov?.Id,
                CityId = defaultCity?.Id,
                Gender = Gender.Male,
                BirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminOptions.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                logger.LogInformation("Admin user '{Email}' seeded successfully.", adminOptions.Email);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("Failed to seed Admin user: {Errors}", errors);
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(existingAdmin, Roles.Admin))
            {
                await userManager.AddToRoleAsync(existingAdmin, Roles.Admin);
                logger.LogInformation("Assigned Admin role to existing user '{Email}'.", adminOptions.Email);
            }
        }
    }
}
