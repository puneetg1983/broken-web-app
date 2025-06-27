using Microsoft.AspNetCore.Mvc;

namespace DiagnosticScenarios.Web.Controllers
{
    public class RestartWebAppController : Controller
    {
        private readonly ILogger<RestartWebAppController> _logger;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public RestartWebAppController(ILogger<RestartWebAppController> logger, IHostApplicationLifetime applicationLifetime)
        {
            _logger = logger;
            _applicationLifetime = applicationLifetime;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("/RestartWebApp/api")]
        public IActionResult Get()
        {
            try
            {
                // Log the restart request
                _logger.LogInformation("Restart requested via RestartWebAppController");
                
                // Start a new thread to handle the exit
                // This ensures the response is sent back to the client before the app exits
                var exitThread = new Thread(() =>
                {
                    // Give a small delay to ensure the response is sent
                    Thread.Sleep(1000);
                    _applicationLifetime.StopApplication();
                });
                
                exitThread.Start();
                
                // Send success response
                return Ok(new { status = "success", message = "Restart initiated" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during restart request");
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }
    }
} 