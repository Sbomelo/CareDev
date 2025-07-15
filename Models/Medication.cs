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
        
        // Navigation property 
        public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    }
}
