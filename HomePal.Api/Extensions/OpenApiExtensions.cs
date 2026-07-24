using Microsoft.OpenApi;

namespace HomePal.Api.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "HomePal API",
                    Version = "v1",
                    Description = "HomePal Authentication & Household Management API"
                };

                var bearerScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT Bearer token"
                };

                var components = document.Components ??= new OpenApiComponents();
                components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                components.SecuritySchemes["Bearer"] = bearerScheme;

                var schemeRef = new OpenApiSecuritySchemeReference("Bearer", document);
                document.Security ??= new List<OpenApiSecurityRequirement>();
                document.Security.Add(new OpenApiSecurityRequirement
                {
                    [schemeRef] = new List<string>()
                });

                return Task.CompletedTask;
            });

            options.AddOperationTransformer((operation, context, cancellationToken) =>
            {
                operation.Parameters ??= new List<IOpenApiParameter>();
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Accept-Language",
                    In = ParameterLocation.Header,
                    Required = false,
                    Description = "Preferred response language (e.g. 'ar-EG', 'ar', 'en-US', 'en')",
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    }
                });
                return Task.CompletedTask;
            });
        });

        return services;
    }
}
