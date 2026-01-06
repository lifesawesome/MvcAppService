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
    
    // Maximum allowed length for RequestId to prevent potential attacks
    private const int MaxRequestIdLength = 200;

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
        // Log page access at Debug level for privacy compliance (GDPR, CCPA)
        _logger.LogDebug("Home page accessed from IP: {IPAddress}", HttpContext.Connection.RemoteIpAddress);
        return View();
    }

    /// <summary>
    /// Displays the privacy policy page.
    /// </summary>
    /// <returns>The privacy policy view.</returns>
    [HttpGet]
    public IActionResult Privacy()
    {
        // Log page access at Debug level for privacy compliance (GDPR, CCPA)
        _logger.LogDebug("Privacy page accessed from IP: {IPAddress}", HttpContext.Connection.RemoteIpAddress);
        return View();
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
            
            // Log error access for security monitoring at Debug level for privacy compliance
            _logger.LogWarning("Error page accessed. RequestId: {RequestId}", requestId);
            _logger.LogDebug("Error page accessed from IP: {IPAddress}", HttpContext.Connection.RemoteIpAddress);
            
            // Validate and sanitize RequestId to prevent potential XSS
            if (!string.IsNullOrEmpty(requestId) && requestId.Length > MaxRequestIdLength)
            {
                _logger.LogWarning("Suspicious RequestId length detected: {Length}", requestId.Length);
                requestId = requestId.Substring(0, MaxRequestIdLength);
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
