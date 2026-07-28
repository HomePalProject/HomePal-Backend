using HomePal.Api.Extensions;
using HomePal.Api.Middleware;
using HomePal.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace HomePal.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices(builder.Configuration);

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<HomePal.Persistence.Context.ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        await RoleSeeder.SeedRolesAsync(app.Services);
        await AdminSeeder.SeedAdminUserAsync(app.Services, builder.Configuration);

        app.UseMiddleware<ExceptionMiddleware>();

        app.MapOpenApi();
        app.MapScalarApiReference();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseCors("AllowAll");

        app.UseRequestLocalization();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}
