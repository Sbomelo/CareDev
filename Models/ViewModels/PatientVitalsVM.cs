using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CareDev.Models.ViewModels
{
    public class PatientVitalsVM
    {
        public int VitalID { get; set; }

        [Required]
        public string PatientUserId { get; set; }
        
        [Required]
        public string NurseUserId { get; set; }

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
        public string BloodPressure { get; set; } // e.g. "120/80"

        [Display(Name = "Oxygen Saturation (%)")]
        [Range(50, 100)]
        public int OxygenSaturation { get; set; }

        [Display(Name = "Glucose Level (mmol/L)")]
        [Range(1.0, 25.0)]
        public double? GlucoseLevel { get; set; }

        public DateTime RecordedDate { get; set; } = DateTime.Now;
    }
}

