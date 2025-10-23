using System.ComponentModel.DataAnnotations;

namespace MvcCrudApp.ViewModels
{
    /// <summary>
    /// View model for string analysis input and results.
    /// </summary>
    public class StringAnalysisViewModel
    {
        /// <summary>
        /// Gets or sets the input strings as a newline-separated text.
        /// </summary>
        [Required(ErrorMessage = "Please enter at least one string")]
        [Display(Name = "Input Strings (one per line)")]
        public string? InputText { get; set; }

        /// <summary>
        /// Gets or sets the largest string found from the input.
        /// </summary>
        [Display(Name = "Largest String")]
        public string? LargestString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the largest string is a palindrome.
        /// </summary>
        [Display(Name = "Is Palindrome")]
        public bool IsPalindrome { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the analysis has been performed.
        /// </summary>
        public bool HasResult { get; set; }
    }
}
