using Microsoft.ApplicationInsights;

namespace MvcCrudApp.Services
{
    /// <summary>
    /// Provides string analysis functionality including finding the largest string and palindrome detection.
    /// </summary>
    public class StringAnalyzerService : IStringAnalyzerService
    {
        private readonly ILogger<StringAnalyzerService> _logger;
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringAnalyzerService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging.</param>
        /// <param name="telemetryClient">The telemetry client for Application Insights.</param>
        public StringAnalyzerService(ILogger<StringAnalyzerService> logger, TelemetryClient telemetryClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
        }

        /// <inheritdoc/>
        public string? GetLargestString(IEnumerable<string>? strings)
        {
            try
            {
                if (strings == null || !strings.Any())
                {
                    _logger.LogWarning("GetLargestString called with null or empty collection");
                    return null;
                }

                var filteredStrings = strings.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                
                if (!filteredStrings.Any())
                {
                    _logger.LogWarning("GetLargestString: All strings were null or whitespace");
                    return null;
                }

                var largest = filteredStrings.OrderByDescending(s => s.Length).First();
                _logger.LogInformation("Found largest string with length: {Length}", largest.Length);
                _telemetryClient.TrackEvent("GetLargestString", new Dictionary<string, string>
                {
                    { "ResultLength", largest.Length.ToString() }
                });
                
                return largest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetLargestString");
                throw;
            }
        }

        /// <inheritdoc/>
        public bool IsPalindrome(string? input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    _logger.LogWarning("IsPalindrome called with null or empty string");
                    return false;
                }

                var cleaned = new string(input.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
                
                if (string.IsNullOrEmpty(cleaned))
                {
                    _logger.LogWarning("IsPalindrome: String contained no alphanumeric characters");
                    return false;
                }

                var reversed = new string(cleaned.Reverse().ToArray());
                var isPalindrome = cleaned == reversed;
                
                _logger.LogInformation("IsPalindrome check: {Result}", isPalindrome);
                _telemetryClient.TrackEvent("IsPalindrome", new Dictionary<string, string>
                {
                    { "Result", isPalindrome.ToString() }
                });
                
                return isPalindrome;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in IsPalindrome");
                throw;
            }
        }

        /// <inheritdoc/>
        public (string? LargestString, bool IsPalindrome) GetLargestStringAndCheckPalindrome(IEnumerable<string>? strings)
        {
            try
            {
                var largestString = GetLargestString(strings);
                var isPalindrome = IsPalindrome(largestString);
                
                _logger.LogInformation("GetLargestStringAndCheckPalindrome - Largest: {Largest}, IsPalindrome: {IsPalindrome}", 
                    largestString ?? "null", isPalindrome);
                
                _telemetryClient.TrackEvent("GetLargestStringAndCheckPalindrome", new Dictionary<string, string>
                {
                    { "HasResult", (largestString != null).ToString() },
                    { "IsPalindrome", isPalindrome.ToString() }
                });
                
                return (largestString, isPalindrome);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetLargestStringAndCheckPalindrome");
                throw;
            }
        }
    }
}
