using System.Text;
using HomePal.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace HomePal.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var userManager = context.HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<HomePal.Domain.Entities.ApplicationUser>>();
                    var userIdClaim = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                                      ?? context.Principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
                    var securityStampClaim = context.Principal?.FindFirst("AspNet.Identity.SecurityStamp")?.Value;

                    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    {
                        context.Fail("Unauthorized: Invalid token claims.");
                        return;
                    }

                    var user = await userManager.FindByIdAsync(userId.ToString());
                    if (user == null || !user.IsActive || user.SecurityStamp != securityStampClaim)
                    {
                        context.Fail("Unauthorized: Token is invalid or user security stamp has changed.");
                        return;
                    }
                }
            };
        });

        return services;
    }
}
