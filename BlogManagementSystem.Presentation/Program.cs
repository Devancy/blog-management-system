using System.Security.Claims;
using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Application.Services;
using BlogManagementSystem.Infrastructure.Persistence;
using BlogManagementSystem.Infrastructure.Services;
using MudBlazor.Services;
using BlogManagementSystem.Presentation.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Refit;
using BlogManagementSystem.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add controllers for the Account Controller
builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options
    .UseNpgsql(
            builder.Configuration.GetConnectionString("Database"),
            npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName))
    .UseSnakeCaseNamingConvention());

// Register repositories
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// Register services
builder.Services.AddScoped<PostService>();

// Add HttpClient factory for connection tests
builder.Services.AddHttpClient();

// Configure Keycloak Admin Client with Refit
builder.Services.AddRefitClient<IKeycloakAdminClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["Keycloak:auth-server-url"]!));

// Register Keycloak Service
builder.Services.AddScoped<IKeycloakService, KeycloakService>();

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    // Ensure proper URL formatting for Keycloak
    var authServerUrl = builder.Configuration["Keycloak:auth-server-url"]!;
    authServerUrl = authServerUrl.EndsWith("/") ? authServerUrl : authServerUrl + "/";
    options.Authority = authServerUrl + "realms/" + builder.Configuration["Keycloak:realm"];
    
    // Make sure we're using the exact client ID from appsettings.json
    options.ClientId = builder.Configuration["Keycloak:resource"]!;
    Console.WriteLine($"Using client ID: {options.ClientId}");
    
    options.ClientSecret = builder.Configuration["Keycloak:credentials:secret"]!;
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
    options.TokenValidationParameters.ValidateAudience = false;
    options.TokenValidationParameters.ValidateIssuer = true;
    options.TokenValidationParameters.ValidIssuer = options.Authority;
    
    options.TokenValidationParameters.NameClaimType = "preferred_username";
    options.TokenValidationParameters.RoleClaimType = "roles";
    
    // Map claims appropriately
    options.ClaimActions.MapJsonKey("roles", "roles");
    options.ClaimActions.MapJsonKey("groups", "groups");
    
    // Ensure metadata URL is correct
    options.MetadataAddress = $"{authServerUrl}realms/{builder.Configuration["Keycloak:realm"]}/.well-known/openid-configuration";
    Console.WriteLine($"Using metadata URL: {options.MetadataAddress}");
    
    // Disable PKCE for troubleshooting
    options.UsePkce = false;
    Console.WriteLine("PKCE is disabled for troubleshooting");
    
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

// Add Authorization policies
builder.Services.AddAuthorization(options => 
{
    options.AddPolicy("RequireAdminRole", policy => 
        policy.RequireRole("Admin"));
    
    options.AddPolicy("RequireEditorRole", policy => 
        policy.RequireRole("Editor"));
    
    options.AddPolicy("RequireAuthorRole", policy => 
        policy.RequireRole("Author"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Apply migrations in development
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

// Add custom authentication middleware for handling redirects
app.UseAuthenticationMiddleware();

app.UseAntiforgery();

// Map controllers before Razor components
app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
