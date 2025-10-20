using CareDev.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Models
{
    public class AdministerMeds
    {
        [Key]
        public int AdministeredId { get; set; }

        // Reference to the dispensed medication record
        [Required]
        public int DispenseId { get; set; }

        [ForeignKey(nameof(DispenseId))]
        public MedicationDispensation MedicationDispensation { get; set; }

        // Who administered the medication (nurse or nursing sister)
        [Required]
        public string AdministeredById { get; set; }   // <-- IdentityUser keys are string, not int
        public ApplicationUser AdministeredBy { get; set; }

        [Required]
        public DateTime TimeGiven { get; set; }

        [MaxLength(500)]
        public string? Observations { get; set; }

        [MaxLength(250)]
        public string? AdverseReactions { get; set; }
    }
}
