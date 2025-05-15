using BlogManagementSystem.Application.Common.Configuration;
using BlogManagementSystem.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlogManagementSystem.Application.Services.Identity;

public class IdentityManagerFactory(IServiceProvider serviceProvider, IdentityConfig config) : IIdentityManagerFactory
{
    private IIdentityManager? _currentManager;

    public IIdentityManager CurrentManager => _currentManager ?? GetManager(config.UseKeycloakAsIdpProxy ? IdentityMode.Proxy : IdentityMode.Keycloak);
    
    public void Initialize(IdentityMode mode)
    {
        config.UseKeycloakAsIdpProxy = mode == IdentityMode.Proxy;
        _currentManager = GetManager(mode);
    }
    
    public IIdentityManager GetManager(IdentityMode mode)
    {
        return mode switch
        {
            IdentityMode.Proxy => serviceProvider.GetRequiredService<ProxyIdentityManager>(),
            IdentityMode.Keycloak => serviceProvider.GetRequiredService<KeycloakIdentityManager>(),
            _ => throw new ArgumentException($"Unsupported identity mode: {mode}")
        };
    }
} 