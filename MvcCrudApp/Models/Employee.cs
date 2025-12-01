using System.ComponentModel.DataAnnotations;

namespace MvcCrudApp.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "Name can only contain letters, spaces, hyphens, and apostrophes")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Department must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Department can only contain letters, numbers, spaces, and hyphens")]
        public string? Department { get; set; }

        [Required(ErrorMessage = "Position is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Position must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Position can only contain letters, numbers, spaces, and hyphens")]
        public string? Position { get; set; }
    }
}
