using System;
using System.Threading.Tasks;
using BlogManagementSystem.Application.Common.Configuration;
using BlogManagementSystem.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlogManagementSystem.Application.Services.Identity;

public class IdentityManagerFactory : IIdentityManagerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IdentityConfig _config;
    private IIdentityManager? _currentManager;
    
    public IdentityManagerFactory(IServiceProvider serviceProvider, IdentityConfig config)
    {
        _serviceProvider = serviceProvider;
        _config = config;
    }
    
    public IIdentityManager CurrentManager => _currentManager ?? GetManager(_config.UseKeycloakAsIdpProxy ? "proxy" : "keycloak");
    
    public async Task Initialize(string mode)
    {
        _config.UseKeycloakAsIdpProxy = mode == "proxy";
        _currentManager = GetManager(mode);
    }
    
    public IIdentityManager GetManager(string mode)
    {
        return mode.ToLower() switch
        {
            "proxy" => _serviceProvider.GetRequiredService<ProxyIdentityManager>(),
            "keycloak" => _serviceProvider.GetRequiredService<KeycloakIdentityManager>(),
            _ => throw new ArgumentException($"Unsupported identity mode: {mode}")
        };
    }
} 