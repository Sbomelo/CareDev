using CareDev.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class MedicationDispensation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PatientUserId { get; set; }   // FK to AspNetUsers.Id
        public ApplicationUser Patient { get; set; }

        [Required]
        public string DispenserUserId { get; set; } // FK to AspNetUsers.Id
        public ApplicationUser Dispenser { get; set; }

        [Required]
        public string MedicationName { get; set; }

        public AdministerMeds? AdministerMeds { get; set; }

        [Required]
        public string Dosage { get; set; }

        public string? Route { get; set; }
        public string? Frequency { get; set; }

        [Required]
        public int ScheduleLevel { get; set; }

        public DateTime TimeDispensed { get; set; } = DateTime.Now;
        public string? Notes { get; set; }
    }

}
