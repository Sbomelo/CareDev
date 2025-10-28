using System.ComponentModel.DataAnnotations;

namespace CareDev.Models.ViewModels
{
    public class UserProfileViewModel
    {
        // "Patient" or "Employee"
        public string EntityKind { get; set; } = string.Empty;

        // PatientId or EmployeeId
        public int EntityId { get; set; }

        // Shared fields
        [Display(Name = "First name")]
        [StringLength(100)]
        public string? Name { get; set; }

        [Display(Name = "Surname")]
        [StringLength(100)]
        public string? SurName { get; set; }

        [Display(Name = "Phone Number")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        [StringLength(10)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Address")]
        [StringLength(250)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        // Patient-specific
        [Display(Name = "Emergency contact name")]
        [StringLength(100)]
        public string? EmergencyContactName { get; set; }

        [Display(Name = "Emergency contact phone")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        [StringLength(10)]
        public string? EmergencyContactPhone { get; set; }

        // Employee-specific (editable only by HR/Admin)
        [Display(Name = "Job title")]
        [StringLength(100)]
        public string? JobTitle { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }
    }
}
