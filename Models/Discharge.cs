using System.ComponentModel.DataAnnotations;
namespace CareDev.Models
{
    public class Discharge
    {
        [Key]
        public int DischargeId { get; set; }

        // Foreign Key
        [Required]
        [Display (Name = "Admission")]
        public int AdmissionId { get; set; }
        [Required]
        [Display (Name = "Patient Name")]
        public int PatientId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Discharge Date")]
        public DateTime DischargeDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Notes { get; set; }



        // Navigation Property
        public virtual Admission? Admission { get; set; } = null!; 
        public virtual Patient? Patient { get; set; } = null!;
    }
}
