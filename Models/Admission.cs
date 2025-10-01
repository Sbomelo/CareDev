//using System.ComponentModel.DataAnnotations;

//namespace CareDev.Models
//{
//    public class Admission
//    {
//        [Key]
//        public int AdmissionId { get; set; }

//        [Required]
//        [DataType(DataType.Date)]
//        public DateTime AdmissionDate { get; set; } = DateTime.Now;

//        [DataType(DataType.Date)]
//        public DateTime? DischargeDate { get; set; } // Nullable to allow for ongoing admissions

//        [Required]
//        [StringLength(500, ErrorMessage = "Admission Reason cannot exceed 500 characters.")]
//        public required string AdmissionReason { get; set; } = string.Empty;

//        //Foreign Keys
//        [Required]
//        [Display (Name ="Patient Name")]
//        public int PatientId { get; set; }  

//        [Required]
//        [Display (Name = "Doctor")]
//        public int? EmployeeId { get; set; }

//        [Required]
//        [Display (Name = "Ward")]
//        public int WardId { get; set; }

//        [Required]
//        [Display (Name = "Bed Number")]
//        public int BedId { get; set; }

//        [Required]
//        [Display (Name = "Room Type")]
//        public string? RoomId { get; set; }

//        // Navigation Proproperties 
//        public Patient Patient { get; set; } 
//        public Employee Employee { get; set; } 
//        public Ward Ward { get; set; } 
//        public Bed Bed { get; set; } 
//        public RoomType Room { get; set; } 




//    }
//}

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
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        // Ward where patient is admitted (required)
        [Required]
        public int WardId { get; set; }
        public Ward Ward { get; set; } = null!;

        // Bed can be assigned later, so nullable
        public int? BedId { get; set; }
        public Bed? Bed { get; set; }

        // Doctor assigned to the admission (optional)
        public int? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        // Admission timestamps
        [Required]
        public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;

        public DateTime? DischargeDate { get; set; }

        // Reason / notes for admission
        [StringLength(500)]
        public string? AdmissionReason { get; set; }

        // Optional free-form notes (doctor/nurse notes)
        public string? Notes { get; set; }

        // Optional convenience flag
        // true = currently admitted, false = discharged (keeps quick checks easy)
        public bool IsActive => DischargeDate == null;
    }
}
