using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class Admission
    {
        [Key]
        public int AdmissionId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AdmissionDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime? DischargeDate { get; set; } // Nullable to allow for ongoing admissions

        [Required]
        [StringLength(500, ErrorMessage = "Admission Reason cannot exceed 500 characters.")]
        public required string AdmissionReason { get; set; } = string.Empty;

        //Foreign Keys
        [Required]
        [Display (Name ="Patient Name")]
        public int PatientId { get; set; }  

        [Required]
        [Display (Name = "Doctor")]
        public int? EmployeeId { get; set; }

        [Required]
        [Display (Name = "Ward")]
        public int WardId { get; set; }

        [Required]
        [Display (Name = "Bed Number")]
        public int BedId { get; set; }

        [Required]
        [Display (Name = "Room Type")]
        public string? RoomId { get; set; }

        // Navigation Proproperties 
        public Patient Patient { get; set; } 
        public Employee Employee { get; set; } 
        public Ward Ward { get; set; } 
        public Bed Bed { get; set; } 
        public RoomType Room { get; set; } 




    }
}
