using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CareDev.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SurName { get; set; } = string.Empty; 

        [Required]
        [Range(0, 120)]
        public int Age { get; set; }

        [Required]
        public int GenderOptionId { get; set; }
        [ForeignKey("GenderOptionID")]
        public virtual ICollection<GenderOption> Gender { get; set; } = new List<GenderOption>(); 

        [Required]
        [Display(Name = "Phone Number", Prompt ="012345678")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits long.")]
        public string PhoneNumber { get; set; } = string.Empty;

        ////Foreign Keys
        //public int? MedicationId { get; set; }
        //public int? AllergyId { get; set; }
        //public int? ChronicConditionId { get; set; }


        ////Navigation Properties and virtual for lazy loading
        //public virtual ICollection<Medication> Medication { get; set; } = new List<Medication>();
        //public virtual ICollection<Allergy> Allergy { get; set; } = new List<Allergy>();
        //public virtual ICollection<ChronicCondition>ChronicCondition { get; set; } = new List<ChronicCondition>();

        //Relationships
        public virtual ICollection<Admission> Admissions { get; set; } = new List<Admission>();
        public virtual ICollection<Medication> Medications { get; set; } = new List<Medication>();
        public virtual ICollection<Allergy> Allergies { get; set; } = new List<Allergy>();
        public virtual ICollection<ChronicCondition> ChronicConditions { get; set; } = new List<ChronicCondition>();
        public virtual ICollection<PatientMovement> Movement { get; set; } = new List<PatientMovement>();
        public virtual ICollection<Discharge> Discharge { get; set; } = new List<Discharge>();
        public virtual PatientFolder? PatientFolder { get; set; } 
        

    }
}
