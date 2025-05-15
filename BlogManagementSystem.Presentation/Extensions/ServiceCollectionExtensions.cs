using System.Security.Claims;
using BlogManagementSystem.Application.Common;
using BlogManagementSystem.Application.Common.Configuration;
using BlogManagementSystem.Application.Common.Security;
using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Application.Services;
using BlogManagementSystem.Application.Services.Identity;
using BlogManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BlogManagementSystem.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            var authServerUrl = configuration["Keycloak:auth-server-url"]!;
            authServerUrl = authServerUrl.EndsWith('/') ? authServerUrl : authServerUrl + "/";
            options.Authority = authServerUrl + "realms/" + configuration["Keycloak:realm"];
            
            options.ClientId = configuration["Keycloak:resource"]!;
            Console.WriteLine($"Using client ID: {options.ClientId}");
            
            options.ClientSecret = configuration["Keycloak:credentials:secret"]!;
            Console.WriteLine($"Client secret configured: {!string.IsNullOrEmpty(options.ClientSecret)}");
            
            options.ResponseType = "code";
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            
            // Standard OIDC scopes
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            
            // Allow HTTP for development - important for local Keycloak instances
            options.RequireHttpsMetadata = false;

            // Configure proper path for redirect - verify this matches what's in Keycloak
            options.CallbackPath = "/signin-oidc";
            Console.WriteLine($"Using callback path: {options.CallbackPath}");
            
            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidIssuer = options.Authority;
            
            options.TokenValidationParameters.NameClaimType = "preferred_username";
            options.TokenValidationParameters.RoleClaimType = "roles";
            
            // Map claims appropriately
            options.ClaimActions.MapJsonKey("roles", "roles");
            options.ClaimActions.MapJsonKey("groups", "groups");
            
            // Ensure metadata URL is correct
            options.MetadataAddress = $"{authServerUrl}realms/{configuration["Keycloak:realm"]}/.well-known/openid-configuration";
            Console.WriteLine($"Using metadata URL: {options.MetadataAddress}");
            
            // Add additional event handlers for debugging
            options.Events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProvider = context =>
                {
                    Console.WriteLine($"Redirecting to identity provider: {context.ProtocolMessage.IssuerAddress}");
                    Console.WriteLine($"OIDC parameters: client_id={context.ProtocolMessage.ClientId}, redirect_uri={context.ProtocolMessage.RedirectUri}, response_type={context.ProtocolMessage.ResponseType}");
                    
                    // Ensure state parameter is set
                    if (string.IsNullOrEmpty(context.ProtocolMessage.State))
                    {
                        context.ProtocolMessage.State = Guid.NewGuid().ToString();
                    }
                    
                    // For debugging, log all parameters
                    foreach (var param in context.ProtocolMessage.Parameters)
                    {
                        Console.WriteLine($"OIDC request parameter: {param.Key}={param.Value}");
                    }
                    
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    Console.WriteLine($"Authentication failed details: {context.Exception}");
                    
                    context.HandleResponse();
                    context.Response.Redirect("/Error/AuthError?errorMessage=" + Uri.EscapeDataString(context.Exception.Message));
                    return Task.CompletedTask;
                },
                OnTokenValidated = async context =>
                {
                    Console.WriteLine("Token validated successfully!");
                    
                    // Process roles and enhanced identity when in proxy mode
                    var identityMappingService = context.HttpContext.RequestServices
                        .GetRequiredService<IIdentityMappingService>();
                    
                    // Process the user claims, adding any local roles and group memberships
                    context.Principal = await identityMappingService.ProcessUserClaimsAsync(context.Principal!);
                    
                    // Extract roles and groups from the token and add them as claims
                    var rolesClaim = context.Principal?.FindFirst("roles");
                    if (rolesClaim != null && !string.IsNullOrEmpty(rolesClaim.Value))
                    {
                        try
                        {
                            var roles = System.Text.Json.JsonSerializer.Deserialize<string[]>(rolesClaim.Value);
                            if (roles != null)
                            {
                                var identity = context.Principal?.Identity as ClaimsIdentity;
                                foreach (var role in roles)
                                {
                                    identity?.AddClaim(new Claim(ClaimTypes.Role, role));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing roles: {ex.Message}");
                        }
                    }
                },
                OnRemoteFailure = context =>
                {
                    Console.WriteLine($"Remote authentication failure: {context.Failure?.Message}");
                    
                    context.HandleResponse();
                    context.Response.Redirect("/Error/AuthError?errorMessage=" + Uri.EscapeDataString(context.Failure?.Message ?? "Unknown authentication error"));
                    return Task.CompletedTask;
                },
                OnAuthorizationCodeReceived = context =>
                {
                    Console.WriteLine("Authorization code received!");
                    return Task.CompletedTask;
                },
                OnTicketReceived = context =>
                {
                    Console.WriteLine("Ticket received!");
                    return Task.CompletedTask;
                },
                OnUserInformationReceived = context =>
                {
                    Console.WriteLine("User information received!");
                    return Task.CompletedTask;
                }
            };
        });
        
        return services;
    }

    public static IServiceCollection AddBlogAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdminRole", policy => 
                policy.RequireRole(RolePermissions.AdminRole))
            .AddPolicy("RequireEditorRole", policy => 
                policy.RequireRole(RolePermissions.EditorRole))
            .AddPolicy("RequireAuthorRole", policy => 
                policy.RequireRole(RolePermissions.AuthorRole));

        return services;
    }
    
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register configuration
        var identityConfig = new IdentityConfig();
        services.AddSingleton(identityConfig);
        
        // Register repositories
        services.AddScoped<ILocalRoleRepository, LocalRoleRepository>();
        services.AddScoped<ILocalGroupRepository, LocalGroupRepository>();
        services.AddScoped<ILocalUserIdentityRepository, LocalUserIdentityRepository>();
        
        // Register identity managers and services
        services.AddScoped<KeycloakIdentityManager>();
        services.AddScoped<KeycloakIdentityManagerAdapter>();
        services.AddScoped<ProxyIdentityManager>();
        services.AddScoped<IIdentityManagerFactory, IdentityManagerFactory>();
        services.AddScoped<IIdentityMappingService, IdentityMappingService>();
        
        return services;
    }
    
    /// <summary>
    /// Initializes identity settings by loading from the database or migrating from configuration.
    /// This method should be called during app startup after the database is ready.
    /// </summary>
    public static async Task InitializeIdentitySettingsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var settingService = scope.ServiceProvider.GetRequiredService<IAppSettingService>();
        var identityConfig = scope.ServiceProvider.GetRequiredService<IdentityConfig>();
        
        var useKeycloakAsIdpProxy = await settingService.GetSettingAsync(Constants.Identity.UseKeycloakAsIdpProxyKey, false);
        
        // Update the identity config with the value from the database
        identityConfig.UseKeycloakAsIdpProxy = useKeycloakAsIdpProxy;
    }
}