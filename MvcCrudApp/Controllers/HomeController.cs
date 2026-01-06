using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcCrudApp.Models;

namespace MvcCrudApp.Controllers;

/// <summary>
/// Home controller handling main application pages.
/// Implements security best practices including logging, error handling, and input validation.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the home page.
    /// </summary>
    /// <returns>The home page view.</returns>
    [HttpGet]
    public IActionResult Index()
    {
        try
        {
            _logger.LogInformation("Home page accessed from IP: {IPAddress}", HttpContext.Connection.RemoteIpAddress);
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error displaying home page");
            return RedirectToAction(nameof(Error));
        }
    }

    /// <summary>
    /// Displays the privacy policy page.
    /// </summary>
    /// <returns>The privacy policy view.</returns>
    [HttpGet]
    public IActionResult Privacy()
    {
        try
        {
            _logger.LogInformation("Privacy page accessed from IP: {IPAddress}", HttpContext.Connection.RemoteIpAddress);
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error displaying privacy page");
            return RedirectToAction(nameof(Error));
        }
    }

    /// <summary>
    /// Displays the error page with request tracking information.
    /// Implements security best practices by sanitizing error information displayed to users.
    /// </summary>
    /// <returns>The error page view with sanitized error information.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet]
    public IActionResult Error()
    {
        try
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            // Log error access for security monitoring
            _logger.LogWarning("Error page accessed. RequestId: {RequestId}, IP: {IPAddress}", 
                requestId, HttpContext.Connection.RemoteIpAddress);
            
            // Validate and sanitize RequestId to prevent potential XSS
            if (!string.IsNullOrEmpty(requestId) && requestId.Length > 200)
            {
                _logger.LogWarning("Suspicious RequestId length detected: {Length}", requestId.Length);
                requestId = requestId.Substring(0, 200);
            }
            
            return View(new ErrorViewModel { RequestId = requestId });
        }
        catch (Exception ex)
        {
            // Fallback error handling - log but don't expose details
            _logger.LogCritical(ex, "Critical error in Error action");
            return View(new ErrorViewModel { RequestId = "Unknown" });
        }
    }
}
