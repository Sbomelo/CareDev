using System.ComponentModel.DataAnnotations;
namespace CareDev.Models
{
    public class DoctorInstruction
    {
        [Key]
        public long InstructionId { get; set; }

        public string PatientId { get; set; }
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        public DateTime InstructionDate { get; set; }

        public string Instructions { get; set; }
        public string Medication { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string AdditionalNotes { get; set; }

        public bool IsCompleted { get; set; } = false;

        // Navigation Properties
        public virtual Patient? Patient { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }

}
