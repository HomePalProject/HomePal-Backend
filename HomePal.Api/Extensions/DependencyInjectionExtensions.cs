namespace HomePal.Api.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseAndIdentityServices(configuration);
        services.AddJwtAuthenticationServices(configuration);
        services.AddCoreServices(configuration);
        services.AddOpenApiDocumentation();

        return services;
    }
}
