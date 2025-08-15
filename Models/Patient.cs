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

       /* [Required]
        public int GenderOptionId { get; set; }
        [ForeignKey("GenderOptionID")]
        public virtual ICollection<GenderOption> Gender { get; set; } = new List<GenderOption>(); */

        [Required]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        [StringLength(10)]
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
        public virtual ICollection<Admission> Admissions { get; set; } = new HashSet<Admission>();
        public virtual ICollection<MedicationAdministration> MedicationAdministrations { get; set; } = new HashSet<MedicationAdministration>();


        //public virtual ICollection<Allergy> Allergies { get; set; } = new List<ChronicCondition>();
        //public virtual Allergy? Allergies { get; set; }
        //public virtual ICollection<Allergy> Allergy { get; set; } = new HashSet<Allergy>();


        //public virtual ICollection<ChronicCondition> ChronicConditions { get; set; } = new List<ChronicCondition>();
        public virtual ICollection<ChronicCondition> ChronicConditions { get; set; } = new HashSet<ChronicCondition>();
        //public virtual ChronicCondition? ChronicConditions { get; set; }
        public virtual ICollection<PatientMovement> Movement { get; set; } = new HashSet<PatientMovement>();
        public virtual ICollection<Discharge> Discharge { get; set; } = new List<Discharge>();
        public virtual PatientFolder? PatientFolder { get; set; }

        public virtual ICollection<Vital> Vitals { get; set; } = new HashSet<Vital>();
        public virtual ICollection<TreatPatient> TreatPatients { get; set; } = new HashSet<TreatPatient>();
        //public virtual ICollection<Medication> Medications { get; set; } = new HashSet<Medication>();
        public virtual ICollection<PatientAllergy> PatientAllergies { get; set; } = new HashSet<PatientAllergy>();
        public virtual ICollection<PatientCondition> PatientConditions { get; set; } = new HashSet<PatientCondition>();
    }
}
