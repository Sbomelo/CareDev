using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Models
{
    public class PatientAllergy
    {
        public int PatientId { get; set; }
        public int AllergyId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;

        [ForeignKey("AllergyId")]
        public virtual Allergy Allergy { get; set; } = null!;
    }
}
