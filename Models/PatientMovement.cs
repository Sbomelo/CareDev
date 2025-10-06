using System.ComponentModel.DataAnnotations;
namespace CareDev.Models
{
    public class PatientMovement
    {
        [Key]
        public int MovementId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Movement Date")]
        public DateTime MovementDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        [Display(Name = "From Ward")]
        public required string From { get; set; } // e.g., "Ward A", "ICU", etc.

        // Foreign Key
        [Required]
        [Display (Name="Patient Name")]
        public int PatientId { get; set; } 

        [Required]
        [Display (Name = "To Ward")]
        public int WardId { get; set; }

        // Navigation Property
        public virtual Patient? Patient { get; set; }
        public virtual Ward? Ward { get; set; }
    }
}
