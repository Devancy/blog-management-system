using Microsoft.AspNetCore.Mvc;

namespace BlogManagementSystem.Presentation.Controllers
{
    [Route("[controller]/[action]")]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult AuthError(string errorMessage = "Authentication error")
        {
            _logger.LogError("Authentication error: {ErrorMessage}", errorMessage);
            
            ViewData["ErrorMessage"] = errorMessage;
            return View();
        }
    }
} 