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

        // Navigation property
        //public Patient Patients { get; set; }  = null!;
        //public ICollection<Patient> Patients { get; set; } = new HashSet<Patient>();
        public virtual ICollection<PatientCondition> PatientConditions { get; set; } = new HashSet<PatientCondition>();
    }
}
