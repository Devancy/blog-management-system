using System.Security.Claims;
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
            // Ensure proper URL formatting for Keycloak
            var authServerUrl = configuration["Keycloak:auth-server-url"]!;
            authServerUrl = authServerUrl.EndsWith("/") ? authServerUrl : authServerUrl + "/";
            options.Authority = authServerUrl + "realms/" + configuration["Keycloak:realm"];
            
            // Make sure we're using the exact client ID from appsettings.json
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
            
            // Audience validation settings - disable for troubleshooting
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
                OnTokenValidated = context =>
                {
                    Console.WriteLine("Token validated successfully!");
                    
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
                    
                    return Task.CompletedTask;
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
                    Console.WriteLine("Authorization code received successfully!");
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    Console.WriteLine("Message received from Keycloak");
                    if (context.ProtocolMessage.Error != null)
                    {
                        Console.WriteLine($"Error in message: {context.ProtocolMessage.Error}");
                        Console.WriteLine($"Error description: {context.ProtocolMessage.ErrorDescription}");
                        Console.WriteLine($"Error URI: {context.ProtocolMessage.ErrorUri}");
                    }
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
                policy.RequireRole("Admin"))
            .AddPolicy("RequireEditorRole", policy => 
                policy.RequireRole("Editor"))
            .AddPolicy("RequireAuthorRole", policy => 
                policy.RequireRole("Author"));

        return services;
    }
}