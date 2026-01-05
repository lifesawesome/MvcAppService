namespace MvcCrudApp.Models
{
    /// <summary>
    /// View model for displaying error information to users.
    /// </summary>
    /// <remarks>
    /// OWASP Security Compliance (A05:2021 - Security Misconfiguration):
    /// This model is intentionally minimal and only exposes non-sensitive information.
    /// - RequestId: Safe to display - used for error tracking correlation only
    /// - Does NOT contain: exception messages, stack traces, database details, or internal state
    /// - Follows secure error handling practices by keeping error details server-side
    /// </remarks>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the request identifier for error tracking and correlation.
        /// </summary>
        /// <remarks>
        /// This identifier is safe to expose to users as it contains no sensitive information.
        /// It corresponds to Activity.Current.Id or HttpContext.TraceIdentifier for telemetry correlation.
        /// </remarks>
        public string? RequestId { get; set; }

        /// <summary>
        /// Gets a value indicating whether the RequestId should be displayed to the user.
        /// </summary>
        /// <value>True if RequestId is not null or empty; otherwise, false.</value>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
