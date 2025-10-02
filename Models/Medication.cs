using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class Medication
    {
        [Key]
        public int MedicationId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        [StringLength(100)]
        public  string Schedule { get; set; } 

        [StringLength(500)]
        [Display(Name = "Usage Notes")] 
        public string? UsageNotes { get; set; }

        
    }
}
