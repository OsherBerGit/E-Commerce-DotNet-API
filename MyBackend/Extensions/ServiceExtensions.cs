using MyBackend.Mappers;
using MyBackend.Mappers.Interfaces;
using MyBackend.Services;
using MyBackend.Services.Interfaces;

namespace MyBackend.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Dependency Injection - Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();

        // Dependency Injection - Mappers
        services.AddScoped<IUserMapper, UserMapper>();
        services.AddScoped<IProductMapper, ProductMapper>();
        services.AddScoped<IPurchaseMapper, PurchaseMapper>();
        services.AddScoped<IReviewMapper, ReviewMapper>();

        return services;
    }
}