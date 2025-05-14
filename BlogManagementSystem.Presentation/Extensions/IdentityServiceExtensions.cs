using BlogManagementSystem.Application.Common.Configuration;
using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Application.Services.Identity;
using BlogManagementSystem.Infrastructure.Repositories;

namespace BlogManagementSystem.Presentation.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register configuration
        var identityConfig = new IdentityConfig
        {
            UseKeycloakAsIdpProxy = configuration.GetValue<bool>("Identity:UseKeycloakAsIdpProxy")
        };
        services.AddSingleton(identityConfig);
        
        // Register repositories
        services.AddScoped<ILocalRoleRepository, LocalRoleRepository>();
        services.AddScoped<ILocalGroupRepository, LocalGroupRepository>();
        services.AddScoped<ILocalUserIdentityRepository, LocalUserIdentityRepository>();
        
        // Register identity managers and services
        services.AddScoped<KeycloakIdentityManager>();
        services.AddScoped<ProxyIdentityManager>();
        services.AddScoped<IIdentityManagerFactory, IdentityManagerFactory>();
        services.AddScoped<IIdentityMappingService, IdentityMappingService>();
        
        return services;
    }
} 