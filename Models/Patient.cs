using Humanizer;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace CareDev.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        [StringLength(50)]
        public required string SurName { get; set; }

        [Required]
        [Range(0, 120)]
        public int Age { get; set; }

        [Required]
        [Display(Name="Phone Number")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        [StringLength(10,MinimumLength =10, ErrorMessage = "Phone number must be 10 digits long.")]
        public string PhoneNumber { get; set; } //= string.Empty; // Initialize to avoid null reference issues

        //Foreign Keys
        public int GenderOptionId { get; set; }
        public int MedicationId { get; set; }
        public int AllergyId { get; set; }
        public int ChronicConditionId { get; set; }


        //Navigation Properties 
        public virtual GenderOption Gender { get; set; } // = new GenderOption(); // Initialize to avoid null reference issues
        public virtual Medication Medication { get; set; }// = new Medication(); // Initialize to avoid null reference issues
        public virtual Allergy Allergy { get; set; } //= new Allergy(); // Initialize to avoid null reference issues
        public virtual ChronicCondition ChronicCondition { get; set; }//= new ChronicCondition();

        //Relationships








    }
}
