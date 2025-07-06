using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class ChronicCondition
    {
        [Key]
        public int ChronicConditionId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        // Navigation property for related patients
        public ICollection<Patient> Patients { get; set; }
    }
}
