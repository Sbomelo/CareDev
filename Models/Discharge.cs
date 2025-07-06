using System.ComponentModel.DataAnnotations;
namespace CareDev.Models
{
    public class Discharge
    {
        [Key]
        public int DischargeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DischargeDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Notes { get; set; } // Additional notes or instructions

        // Foreign Key
        [Required]
        public int AdmissionId { get; set; }

        // Navigation Property
        public virtual Admission Admission { get; set; } = null!; // Non-nullable reference type, must be initialized
        public virtual Patient Patient { get; set; } = null!; // Non-nullable reference type, must be initialized
    }
}
