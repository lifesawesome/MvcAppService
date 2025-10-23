namespace MvcCrudApp.Services
{
    /// <summary>
    /// Interface for string analysis operations.
    /// </summary>
    public interface IStringAnalyzerService
    {
        /// <summary>
        /// Gets the largest string from a collection of strings.
        /// </summary>
        /// <param name="strings">The collection of strings to analyze.</param>
        /// <returns>The longest string from the collection, or null if the collection is empty or null.</returns>
        string? GetLargestString(IEnumerable<string>? strings);

        /// <summary>
        /// Checks if a string is a palindrome (reads the same forwards and backwards).
        /// </summary>
        /// <param name="input">The string to check.</param>
        /// <returns>True if the string is a palindrome; otherwise, false.</returns>
        bool IsPalindrome(string? input);

        /// <summary>
        /// Gets the largest string from a collection and checks if it is a palindrome.
        /// </summary>
        /// <param name="strings">The collection of strings to analyze.</param>
        /// <returns>A tuple containing the largest string and a boolean indicating if it is a palindrome.</returns>
        (string? LargestString, bool IsPalindrome) GetLargestStringAndCheckPalindrome(IEnumerable<string>? strings);
    }
}
