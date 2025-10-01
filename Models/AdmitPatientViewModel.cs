using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class AdmitPatientViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SurName { get; set; }

        public int? Age { get; set; }

        [Required]
        public string Gender { get; set; }

        [Display(Name = "Medication")]
        public int? MedicationId { get; set; }

        [Display(Name = "Allergy")]
        public int? AllergyId { get; set; }

        [Display(Name = "Chronic Condition")]
        public int?ChronicConditionId { get; set; }

        [Display (Name = "Ward")]
        public int? WardId { get; set; }

        [Display(Name = "Bed")]
        public int? BedId { get; set; }

        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Admission Reason cannot exceed 500 characters.")]
        [Display(Name = "Admission Reason")]
        public string? AdmissionReason { get; set; }

        public IEnumerable<SelectListItem>? Genders { get; set; }
        public IEnumerable<SelectListItem>? Allergies { get; set; }
        public IEnumerable<SelectListItem>? Medications { get; set; }
        public IEnumerable<SelectListItem>? ChronicConditions { get; set; }
        public IEnumerable<SelectListItem>? Wards { get; set; }
        public IEnumerable<SelectListItem>? Beds { get; set; }
        public IEnumerable<SelectListItem>? Employees { get; set; } 


    }
}
