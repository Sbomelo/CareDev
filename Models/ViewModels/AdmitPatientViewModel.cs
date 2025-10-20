using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CareDev.Models.ViewModels
{
    public class AdmitPatientViewModel
    {
        // Patient to admit (required)
        [Required]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        // Admitting staff (optional)
        [Display(Name = "Admitting Staff")]
        public int? EmployeeId { get; set; }

        // Ward selection (required)
        [Required]
        [Display(Name = "Ward")]
        public int WardId { get; set; }

        // Bed selection (optional)
        [Display(Name = "Bed")]
        public int? BedId { get; set; }

        // Doctor assignment (optional)
        [Display(Name = "Doctor")]
        public int? DoctorId { get;set; }

        [Display(Name = "Admission Date")]
        public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Admission Reason")]
        [StringLength(500)]
        public string? AdmissionReason { get; set; }

        // Dropdown collections for the view
        public IEnumerable<SelectListItem>? Patients { get; set; }
        public IEnumerable<SelectListItem>? Employees { get; set; }
        public IEnumerable<SelectListItem>? Wards { get; set; }
        public IEnumerable<SelectListItem>? Beds { get; set; }    // available beds
        public IEnumerable<SelectListItem>? Doctors { get; set; }
    }
}
