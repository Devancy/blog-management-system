namespace BlogManagementSystem.Presentation.Middleware
{
    public class AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            await next(context);

            // Check if we have an unauthorized response and user is not authenticated
            if (context.Response.StatusCode == 401 && context.User.Identity is { IsAuthenticated: false })
            {
                logger.LogInformation("Unauthorized request. Redirecting to login page.");
                
                // Save the current URI so we can redirect back after login
                var currentUri = context.Request.Path + context.Request.QueryString;
                var encodedRedirectUri = Uri.EscapeDataString(currentUri);
                
                // Redirect to login
                context.Response.Redirect($"/Account/Login?returnUrl={encodedRedirectUri}");
                return;
            }
            
            // Check for forbidden response
            if (context.Response.StatusCode == 403)
            {
                logger.LogInformation("Forbidden request. Redirecting to access denied page.");
                
                // Redirect to access denied page
                context.Response.Redirect("/Account/AccessDenied");
                return;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
} 