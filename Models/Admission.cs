using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Models
{
    public class Admission
    {
        [Key]
        public int AdmissionId { get; set; }

        // The patient being admitted (required)
        [Required]
        [Display (Name="Patient Name")]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; } = null!;

        // Ward where patient is admitted (required)
        [Required]
        [Display (Name="Ward Name")]
        public int WardId { get; set; }
        public Ward? Ward { get; set; } =null!;

        // Bed can be assigned later, so nullable
        [Display (Name="Bed Number")]
        public int? BedId { get; set; }
        public Bed? Bed { get; set; }

        // Doctor assigned to the admission (optional)
        [Display(Name = "Doctor Name")]
        public int? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [Display(Name="Employee Name")]
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        // Admission timestamps
        [Required]
        [Display (Name ="Admission Date")]
        public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Discharge Date")]
        public DateTime? DischargeDate { get; set; }

        // Reason / notes for admission
        [Required]
        [StringLength(500)]
        public string? AdmissionReason { get; set; }

        // Optional free-form notes (doctor/nurse notes)
        public string? Notes { get; set; }

        // Optional convenience flag
        // true = currently admitted, false = discharged (keeps quick checks easy)
        public bool IsActive { get; set; } = true;
    }
}
