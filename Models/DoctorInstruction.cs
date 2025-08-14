using CareDev.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CareDev.Models
{
    public class DoctorInstruction
    {
        [Key]
        public long InstructionId { get; set; }
        [Required]
        public long PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        [Display(Name = "Doctor Name")]
        public string UserId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Instruction Date")]
        public DateTime InstructionDate { get; set; }

        [Required]
        [Display(Name = "Medical Instructions")]
        [DataType(DataType.MultilineText)]
        public string Instructions { get; set; }

        [Display(Name = "Medication Prescribed")]
        public string Medication { get; set; }

        [Display (Name = "Follow-up Date")]
        [DataType(DataType.Date)]
        public DateTime? FollowUpDate { get; set; }

        [Display(Name = "Additional Notes")]
        [DataType(DataType.MultilineText)]
        public string AdditionalNotes { get; set; } 
    }
}
