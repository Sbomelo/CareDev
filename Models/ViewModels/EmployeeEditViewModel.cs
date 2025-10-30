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
        //Date of birth
        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [Display(Name = "ID Number")]
        [StringLength(13, ErrorMessage = "ID Number Must be 13 digits")]
        public string IDNumber { get; set; } = string.Empty;
        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Address")]
        [StringLength(250)]
        public string? Address { get; set; }

        [Required]
        [StringLength(50)]
        public string? City { get; set; }

        [Required, RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Job title")]
        [StringLength(100)]
        public string? JobTitle { get; set; }

        [Required]
        [StringLength(100)]
        public string? Department { get; set; }
        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        // Select lists for the view
        public IEnumerable<SelectListItem>? Roles { get; set; }

    }
}

