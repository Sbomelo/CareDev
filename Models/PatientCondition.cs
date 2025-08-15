using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Models
{
    public class PatientCondition
    {
        public int PatientId { get; set; }
        public int ChronicConditionId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;

        [ForeignKey("ChronicConditionId")]
        public virtual ChronicCondition ChronicCondition { get; set; } = null!;
    }
}
