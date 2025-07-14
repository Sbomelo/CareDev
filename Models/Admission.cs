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

        //Foreign Keys
        [Required]
        public int PatientId { get; set; }  
        [Required]
        public int? DoctorId { get; set; }
        [Required]
        public int WardId { get; set; }
        [Required]
        public int BedId { get; set; }
        //[Required]
        //public string? RoomId { get; set; } 

        // Navigation Proproperties 
        public virtual Patient Patient { get; set; } = null!; // Non-nullable reference type, must be initialized
        public virtual Doctor Doctor { get; set; } = null!; // Non-nullable reference type, must be initialized
        public virtual Ward Ward { get; set; } = null!; // Non-nullable reference type, must be initialized
        public virtual Bed Bed { get; set; } = null!; // Non-nullable reference type, must be initialized




    }
}
