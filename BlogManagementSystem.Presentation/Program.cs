using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Application.Services;
using BlogManagementSystem.Infrastructure.Persistence;
using BlogManagementSystem.Infrastructure.Repositories;
using BlogManagementSystem.Infrastructure.Services;
using MudBlazor.Services;
using BlogManagementSystem.Presentation.Components;
using BlogManagementSystem.Presentation.Extensions;
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
builder.Services.AddScoped<IAppSettingRepository, AppSettingRepository>();

// Register services
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<AppSettingService>();

// Add HttpClient factory for connection tests
builder.Services.AddHttpClient();

// Configure Keycloak Admin Client with Refit
builder.Services.AddRefitClient<IKeycloakAdminClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["Keycloak:auth-server-url"]!));

// Register Keycloak Service
builder.Services.AddScoped<IKeycloakService, KeycloakService>();

// Register Identity Services
builder.Services.AddIdentityServices(builder.Configuration);

// Configure Authentication and Authorization
builder.Services.AddKeycloakAuthentication(builder.Configuration);
builder.Services.AddBlogAuthorization();

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

// Initialize identity settings from database
await app.Services.InitializeIdentitySettingsAsync();

app.Run();
