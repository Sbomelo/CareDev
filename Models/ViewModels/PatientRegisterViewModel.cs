using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace CareDev.Models.ViewModels
{
    public class PatientRegisterViewModel
    {
        [Required, StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required, EmailAddress]
        [Display(Name = "Email Adress")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required,Range(0,100)]
        public int Age { get; set; }

        [Required]
        [Display(Name = "ID Number")]
        [StringLength(13, ErrorMessage = "ID Number Must be 13 digits")]
        public string IDNumber { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Address")]
        [StringLength(250)]
        public string? Address { get; set; }

        [Required]
        [StringLength(50)]
        public string? City { get; set; }

        [Required]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        [Display(Name ="Meidcation")]
        public int? MedicationId { get; set; }

        [Display(Name = "Allergy")]
        public int? AllergyId { get; set; }

        [Display(Name = "Chronic Condition")]
        public int? ChronicConditionId { get; set; }

        [Required, DataType(DataType.Password)]
        [StringLength(100, MinimumLength =6, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }


        // For dropdown lists
        public IEnumerable<SelectListItem>? Medications { get; set; }
        public IEnumerable<SelectListItem>? Allergies { get; set; }
        public IEnumerable<SelectListItem>? ChronicConditions { get; set; }
    }
}
