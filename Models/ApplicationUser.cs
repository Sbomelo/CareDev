using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "SurName")]
        public string SurName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Age")]
        public int Age { get; set; } 


        [Required]
        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        public int? MedicationId { get; set; }
        public int? AllergyId { get; set; }
        public int? ChronicConditionId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(MedicationId))]
        public virtual Medication? Medication { get; set; }

        [ForeignKey(nameof(AllergyId))]
        public virtual Allergy? Allergy { get; set; }

        [ForeignKey(nameof(ChronicConditionId))]
        public virtual ChronicCondition? ChronicCondition { get; set; }

        public bool MustChangePassword { get; set; } = false;

        //Navigation property for Patient profile (if applicable)
        public Patient? Patient { get; set; }
    }
}
