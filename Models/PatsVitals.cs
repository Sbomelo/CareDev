using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Models
{
    public class PatsVitals
    {
        [Key]
        public int Id { get; set; }

        // 🔹 Foreign key to Patient (ApplicationUser in role "Patient")
        [Required]
        [ForeignKey(nameof(Patient))]
        public string PatientUserId { get; set; } = string.Empty;
        public ApplicationUser Patient { get; set; } = null!;

        // 🔹 Foreign key to Nurse (ApplicationUser in role "Nurse")
        [Required]
        [ForeignKey(nameof(Nurse))]
        public string NurseUserId { get; set; } = string.Empty;
        public ApplicationUser Nurse { get; set; } = null!;

        // 🔹 Vital signs
        [Required]
        [Display(Name = "Temperature (°C)")]
        [Range(30.0, 45.0)]
        public double Temperature { get; set; }

        [Required]
        [Display(Name = "Heart Rate (bpm)")]
        [Range(30, 200)]
        public int HeartRate { get; set; }

        [Required]
        [Display(Name = "Respiratory Rate (breaths/min)")]
        [Range(10, 50)]
        public int RespiratoryRate { get; set; }

        [Required]
        [Display(Name = "Blood Pressure")]
        public string BloodPressure { get; set; } = string.Empty; // e.g. "120/80"

        [Display(Name = "Oxygen Saturation (%)")]
        [Range(50, 100)]
        public int OxygenSaturation { get; set; }

        [Display(Name = "Glucose Level (mmol/L)")]
        [Range(1.0, 25.0)]
        public double? GlucoseLevel { get; set; }

        public DateTime RecordedDate { get; set; } = DateTime.Now;
    }
}
