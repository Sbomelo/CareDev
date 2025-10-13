using System.ComponentModel.DataAnnotations;

namespace CareDev.Models.ViewModels
{
    public class MedicationDispensationVM
    {
        [Required]
        public string PatientUserId { get; set; }

        [Required]
        public string MedicationName { get; set; }

        [Required]
        public string Dosage { get; set; }

        public string Route { get; set; }

        public string Frequency { get; set; }

        [Required]
        [Range(1, 7)]
        public int ScheduleLevel { get; set; }

        public string? Notes { get; set; }
        public DateTime DispenseDate { get; internal set; }
        //public User DispenserUserId { get; internal set; }
    }
}
