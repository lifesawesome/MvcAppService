//Generate an error view model class in the Models folder. This class will be used to display error messages in the views.
namespace MvcCrudApp.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
