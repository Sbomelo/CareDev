using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;


namespace CareDev.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required, StringLength(30)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string SurName { get; set; } = string.Empty;

        [Range(18, 100)]
        public int? Age { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required, RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        // Select lists for the view
        public IEnumerable<SelectListItem>? Roles { get; set; }

    }
}

