using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Models
{
    public class MedicationAdministration
    {
        [Key]
        public int Admin_ID { get; set; }

        [Required]  
        public int PatientId { get; set; }

        [Required]
        public int MedicationId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Dosage { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Time { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        //navaigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("MedicationId")]
        public virtual Medication Medication { get; set; }
    }
}
