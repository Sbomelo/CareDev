using System.ComponentModel.DataAnnotations;
namespace CareDev.Models
{
    public class PatientMovement
    {
        [Key]
        public int MovementId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime MovementDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        public required string Location { get; set; } // e.g., "Ward A", "ICU", etc.

        // Foreign Key
        [Required]
        public int PatientId { get; set; } 
        [Required]
        public int WardId { get; set; }

        // Navigation Property
        public virtual Patient Patient { get; set; } = null!;
        public virtual Ward Ward { get; set; } = null!; 
    }
}
