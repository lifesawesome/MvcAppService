using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcCrudApp.Models;

namespace MvcCrudApp.Controllers;

/// <summary>
/// Home controller provides core application views including landing page, privacy policy, and error handling.
/// All actions are read-only and do not process user input or perform state changes.
/// </summary>
/// <remarks>
/// OWASP Security Compliance:
/// - No user input processing (low XSS/injection risk)
/// - No state-changing actions (CSRF protection not required)
/// - Error handling follows secure practices (no sensitive data exposure)
/// - Public access appropriate for informational content
/// - HTTPS enforced globally in Program.cs
/// </remarks>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of the HomeController with dependency injection.
    /// </summary>
    /// <param name="logger">Logger instance for structured logging of application events.</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the application home page.
    /// </summary>
    /// <returns>View result for the home page.</returns>
    /// <remarks>
    /// Security: Public access, no authentication required for landing page.
    /// No user input processed, no data persistence operations.
    /// 
    /// Privacy Note: IP addresses are logged for security monitoring purposes only.
    /// Ensure this logging complies with your privacy policy and data retention requirements.
    /// Consider anonymizing or hashing IP addresses if full address is not required.
    /// </remarks>
    public IActionResult Index()
    {
        // Log page access for security monitoring (consider privacy implications)
        _logger.LogInformation("Home page accessed from IP: {IpAddress}", 
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
        
        return View();
    }

    /// <summary>
    /// Displays the privacy policy page.
    /// </summary>
    /// <returns>View result for the privacy policy page.</returns>
    /// <remarks>
    /// Security: Public access, static informational content.
    /// No user input processed or sensitive data displayed.
    /// </remarks>
    public IActionResult Privacy()
    {
        _logger.LogInformation("Privacy policy page accessed");
        
        return View();
    }

    /// <summary>
    /// Displays application error information in a user-friendly format.
    /// </summary>
    /// <returns>View result with minimal error context (RequestId only).</returns>
    /// <remarks>
    /// OWASP Security Compliance (A05:2021 - Security Misconfiguration):
    /// - Does NOT expose stack traces or detailed error messages to users
    /// - Only displays non-sensitive RequestId for error tracking
    /// - Detailed errors logged server-side only via ILogger
    /// - Response caching disabled to prevent stale error pages
    /// - Activity.Current.Id provides correlation for Application Insights telemetry
    /// 
    /// Security Best Practices:
    /// - RequestId and TraceIdentifier are safe to expose (non-sensitive identifiers)
    /// - Error details written to secure logging infrastructure only
    /// - No database connection strings, file paths, or internal state exposed
    /// - Complies with OWASP logging best practices (A09:2021)
    /// </remarks>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        
        // Log error occurrence for security monitoring
        _logger.LogWarning("Error page displayed. RequestId: {RequestId}, Path: {Path}", 
            requestId, 
            HttpContext.Request.Path);
        
        return View(new ErrorViewModel { RequestId = requestId });
    }
}
