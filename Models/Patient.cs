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
        [StringLength(100)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        [StringLength(10)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Address")]
        [StringLength(250)]
        public string? Address { get; set; }
        
        [Required]
        [StringLength(50)]
        public string? City { get; set; }

        //Foreign Keys
        [Display(Name="Medication")]
        public int? MedicationId { get; set; }
        public Medication? Medications { get; set; }

        [Display(Name = "Allergy")]
        public int? AllergyId { get; set; }
        public Allergy? Allergy { get; set; }

        [Display(Name = "Chronic Condition")]
        public int? ChronicConditionId { get; set; }
        public ChronicCondition? ChronicCondition { get; set; }

        [Display(Name = "Emergency contact name")]
        [StringLength(100)]
        public string? EmergencyContactName { get; set; }

        [Display(Name = "Emergency contact phone")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        [StringLength(10)]
        public string? EmergencyContactPhone { get; set; }

        //Indicates if the patient is currently admitted
        public bool IsAdmitted { get; set; } = false;

        //Link to identity user
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }


        //Relationships
        public virtual ICollection<Admission> Admissions { get; set; } = new HashSet<Admission>();
        public virtual ICollection<MedicationAdministration> MedicationAdministrations { get; set; } = new HashSet<MedicationAdministration>();
        public virtual ICollection<PatientMovement> Movement { get; set; } = new HashSet<PatientMovement>();
        public virtual ICollection<Discharge> Discharge { get; set; } = new List<Discharge>();
        public virtual PatientFolder? PatientFolder { get; set; }
        public virtual ICollection<Vital> Vitals { get; set; } = new HashSet<Vital>();
        public virtual ICollection<TreatPatient> TreatPatients { get; set; } = new HashSet<TreatPatient>();
        public virtual ICollection<PatientAllergy> PatientAllergies { get; set; } = new HashSet<PatientAllergy>();
        public virtual ICollection<PatientCondition> PatientConditions { get; set; } = new HashSet<PatientCondition>();
    }
}
