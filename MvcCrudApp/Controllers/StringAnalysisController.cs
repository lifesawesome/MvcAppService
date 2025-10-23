using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;
using MvcCrudApp.Services;
using MvcCrudApp.ViewModels;

namespace MvcCrudApp.Controllers
{
    /// <summary>
    /// Controller for string analysis operations.
    /// </summary>
    public class StringAnalysisController : Controller
    {
        private readonly IStringAnalyzerService _stringAnalyzerService;
        private readonly ILogger<StringAnalysisController> _logger;
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringAnalysisController"/> class.
        /// </summary>
        /// <param name="stringAnalyzerService">The string analyzer service.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="telemetryClient">The telemetry client.</param>
        public StringAnalysisController(
            IStringAnalyzerService stringAnalyzerService,
            ILogger<StringAnalysisController> logger,
            TelemetryClient telemetryClient)
        {
            _stringAnalyzerService = stringAnalyzerService ?? throw new ArgumentNullException(nameof(stringAnalyzerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
        }

        /// <summary>
        /// Displays the string analysis form.
        /// </summary>
        /// <returns>The index view.</returns>
        public IActionResult Index()
        {
            _telemetryClient.TrackEvent("StringAnalysisPageVisited");
            _logger.LogInformation("String Analysis Index page accessed");
            return View(new StringAnalysisViewModel());
        }

        /// <summary>
        /// Processes the string analysis request.
        /// </summary>
        /// <param name="model">The view model containing input strings.</param>
        /// <returns>The index view with analysis results.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(StringAnalysisViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in StringAnalysis POST");
                return View(model);
            }

            try
            {
                if (string.IsNullOrWhiteSpace(model.InputText))
                {
                    ModelState.AddModelError(nameof(model.InputText), "Please enter at least one string");
                    return View(model);
                }

                var strings = model.InputText
                    .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

                if (!strings.Any())
                {
                    ModelState.AddModelError(nameof(model.InputText), "Please enter at least one non-empty string");
                    return View(model);
                }

                var (largestString, isPalindrome) = _stringAnalyzerService.GetLargestStringAndCheckPalindrome(strings);

                model.LargestString = largestString;
                model.IsPalindrome = isPalindrome;
                model.HasResult = true;

                _logger.LogInformation("String analysis completed successfully. Largest string length: {Length}, IsPalindrome: {IsPalindrome}", 
                    largestString?.Length ?? 0, isPalindrome);

                _telemetryClient.TrackEvent("StringAnalysisCompleted", new Dictionary<string, string>
                {
                    { "InputCount", strings.Count.ToString() },
                    { "IsPalindrome", isPalindrome.ToString() }
                });

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during string analysis");
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again.");
                return View(model);
            }
        }
    }
}
