using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagementSystem.Presentation.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly string _keycloakRealm;
        private readonly string _keycloakBaseUrl;

        public AccountController(IConfiguration configuration)
        {
            // Get Keycloak configuration from appsettings.json
            _keycloakRealm = configuration["Keycloak:realm"]!;
            
            // Ensure proper URL formatting for Keycloak
            _keycloakBaseUrl = configuration["Keycloak:auth-server-url"]!;
            _keycloakBaseUrl = _keycloakBaseUrl.EndsWith("/") ? _keycloakBaseUrl : _keycloakBaseUrl + "/";
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            // Store return URL for post-login redirect
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = returnUrl,
                IsPersistent = true
            }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            // First, sign out from the local application
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Then sign out from the OIDC provider (Keycloak)
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = returnUrl,
                    IsPersistent = true
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme
            );
        }

        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            // Redirect to Keycloak Account Management
            var accountUrl = $"{_keycloakBaseUrl}realms/{_keycloakRealm}/account";
            return Redirect(accountUrl);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
} 