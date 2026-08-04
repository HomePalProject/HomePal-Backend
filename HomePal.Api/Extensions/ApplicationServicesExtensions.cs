using System.Text.Json.Serialization;
using HomePal.Api.Factories;
using HomePal.Api.Resources;
using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Auth.Interfaces;
using HomePal.Application.Features.Auth.Options;
using HomePal.Application.Features.Auth.Services;
using HomePal.Infrastructure.Authentication;
using HomePal.Infrastructure.Email;
using HomePal.Infrastructure.Google;
using HomePal.Persistence;
using HomePal.Persistence.Repositories;

namespace HomePal.Api.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<MailKitOptions>()
            .Bind(configuration.GetSection(MailKitOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<GoogleAuthOptions>()
            .Bind(configuration.GetSection(GoogleAuthOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<ClientOptions>()
            .Bind(configuration.GetSection(ClientOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<AdminOptions>()
            .Bind(configuration.GetSection(AdminOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<HomePal.Application.Features.HouseholdManagement.Interfaces.IHouseholdRepository, HouseholdRepository>();
        services.AddScoped<HomePal.Application.Features.HouseholdManagement.Interfaces.IHouseholdMemberRepository, HouseholdMemberRepository>();
        services.AddScoped<HomePal.Application.Features.HouseholdManagement.Interfaces.IHouseholdInvitationRepository, HouseholdInvitationRepository>();
        services.AddScoped<HomePal.Application.Features.HouseholdManagement.Interfaces.IPreferenceRepository, PreferenceRepository>();
        services.AddScoped<HomePal.Application.Features.HouseholdManagement.Interfaces.IPreferenceCategoryRepository, PreferenceCategoryRepository>();
        services.AddScoped<HomePal.Application.Features.Catalog.Interfaces.IProductCategoryRepository, ProductCategoryRepository>();
        services.AddScoped<HomePal.Application.Features.Catalog.Interfaces.IMeasuringUnitRepository, MeasuringUnitRepository>();
        services.AddScoped<HomePal.Application.Features.Catalog.Interfaces.ISupermarketRepository, SupermarketRepository>();
        services.AddScoped<HomePal.Application.Features.Catalog.Interfaces.ICanonicalProductRepository, CanonicalProductRepository>();
        services.AddScoped<HomePal.Application.Features.Catalog.Interfaces.IOfferRepository, OfferRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IEmailSender, MailKitEmailSender>();
        services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();
        services.AddScoped<IFileStorageService, HomePal.Infrastructure.Storage.FileStorageService>();

        services.AddLocalization();
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { "en-US", "en", "ar-EG", "ar" };
            options.SetDefaultCulture("en-US")
                   .AddSupportedCultures(supportedCultures)
                   .AddSupportedUICultures(supportedCultures);
        });

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<HomePal.Application.Features.HouseholdManagement.Interfaces.IHouseholdService, HomePal.Application.Features.HouseholdManagement.Services.HouseholdService>();
        services.AddScoped<HomePal.Application.Features.HouseholdManagement.Interfaces.IHouseholdMemberService, HomePal.Application.Features.HouseholdManagement.Services.HouseholdMemberService>();
        services.AddScoped<HomePal.Application.Features.HouseholdManagement.Interfaces.IHouseholdInvitationService, HomePal.Application.Features.HouseholdManagement.Services.HouseholdInvitationService>();
        services.AddScoped<HomePal.Application.Features.HouseholdManagement.Interfaces.IPreferenceService, HomePal.Application.Features.HouseholdManagement.Services.PreferenceService>();
        services.AddScoped<HomePal.Application.Features.HouseholdManagement.Interfaces.IPreferenceCategoryService, HomePal.Application.Features.HouseholdManagement.Services.PreferenceCategoryService>();
        services.AddScoped<HomePal.Application.Features.HouseholdManagement.Interfaces.IMemberPreferenceService, HomePal.Application.Features.HouseholdManagement.Services.MemberPreferenceService>();
        services.AddScoped<HomePal.Application.Features.Catalog.Interfaces.IProductCategoryService, HomePal.Application.Features.Catalog.Services.ProductCategoryService>();
        services.AddScoped<HomePal.Application.Features.Catalog.Interfaces.IMeasuringUnitService, HomePal.Application.Features.Catalog.Services.MeasuringUnitService>();
        services.AddScoped<HomePal.Application.Features.Catalog.Interfaces.ISupermarketService, HomePal.Application.Features.Catalog.Services.SupermarketService>();
        services.AddScoped<HomePal.Application.Features.Catalog.Interfaces.ICanonicalProductService, HomePal.Application.Features.Catalog.Services.CanonicalProductService>();
        services.AddScoped<HomePal.Application.Features.Catalog.Interfaces.IOfferService, HomePal.Application.Features.Catalog.Services.OfferService>();

        services.AddControllers()
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResource));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory.ProduceResponse;
            });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }
}
