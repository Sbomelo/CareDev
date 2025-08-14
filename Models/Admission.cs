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
        public int PatientId { get; set; }  
        [Required]
        public int? EmployeeId { get; set; }
        [Required]
        public int WardId { get; set; }
        [Required]
        public int BedId { get; set; }
        [Required]
        public string? RoomId { get; set; }

        // Navigation Proproperties 
        public virtual Patient Patient { get; set; } 
        public virtual Employee Employee { get; set; } 
        public virtual Ward Ward { get; set; } 
        public virtual Bed Bed { get; set; } 
        public virtual RoomType Room { get; set; } 




    }
}
