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

        public int? PatientId { get; set; }
        public int? EmployeeId { get; set; }

        // Navigation property 
       // public ICollection<Patient> Patients { get; set; } = new List<Patient>();
        //public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public virtual ICollection<MedicationAdministration> MedicationAdministrations { get; set; } = new HashSet<MedicationAdministration>();
    }
}
