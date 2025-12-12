using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvcCrudApp.Data;
using MvcCrudApp.Models;

namespace MvcCrudApp.Controllers
{
    // NOTE: [Authorize] attribute should be added once authentication is configured
    // For now, omitting to ensure application works without authentication setup
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SearchController> _logger;

        public SearchController(ApplicationDbContext context, ILogger<SearchController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Search
        public IActionResult Index()
        {
            _logger.LogInformation("Search page accessed at {Time}", DateTime.UtcNow);
            return View();
        }

        // POST: Search/Results
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Results(string searchTerm)
        {
            try
            {
                // Input validation
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    _logger.LogWarning("Search attempted with empty search term at {Time}", DateTime.UtcNow);
                    ModelState.AddModelError("searchTerm", "Please enter a search term.");
                    return View("Index");
                }

                if (searchTerm.Length < 2)
                {
                    _logger.LogWarning("Search attempted with search term length {Length} at {Time}", searchTerm.Length, DateTime.UtcNow);
                    ModelState.AddModelError("searchTerm", "Search term must be at least 2 characters long.");
                    return View("Index");
                }

                // Sanitize input to prevent XSS (additional safety layer)
                searchTerm = searchTerm.Trim();

                _logger.LogInformation("Search performed with term: {SearchTerm} at {Time}", searchTerm, DateTime.UtcNow);

                // Parameterized query via LINQ (EF Core) - protects against SQL injection
                // Using EF.Functions.Like for case-insensitive search
                var results = await _context.Employees
                    .AsNoTracking()
                    .Where(e => (e.Name != null && EF.Functions.Like(e.Name, $"%{searchTerm}%")) || 
                                (e.Department != null && EF.Functions.Like(e.Department, $"%{searchTerm}%")) || 
                                (e.Position != null && EF.Functions.Like(e.Position, $"%{searchTerm}%")))
                    .OrderBy(e => e.Name)
                    .ToListAsync();

                _logger.LogInformation("Search completed. Found {Count} results for term: {SearchTerm}", results.Count, searchTerm);

                ViewData["SearchTerm"] = searchTerm;
                return View(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during search with term: {SearchTerm} at {Time}", searchTerm, DateTime.UtcNow);
                ModelState.AddModelError("", "An error occurred while processing your search. Please try again.");
                return View("Index");
            }
        }
    }
}
