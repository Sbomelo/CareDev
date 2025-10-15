using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CareDev.Models
{
    public class PatientMovement
    {
        [Key]
        public int MovementId { get; set; }

        [Required]
        [Display(Name =" Admission Number")]
        public int AdmissionId { get; set; }
        
        [Required]
        [Display (Name="Patient Name")]
        public int PatientId { get; set; } 
        
        public int? FromWardId{ get; set; }

        [ForeignKey("FromWardId")]
        public Ward? FromWard { get; set; }

        public int?FromBedId { get; set; }

        [ForeignKey("FromBedId")]
        public Bed? FromBed { get; set; }

        [Required]
        public int ToBedId { get; set; }

        [ForeignKey("ToBedId")]
        public Bed? ToBed { get; set; }

        [Required]
        public int ToWardId { get; set; }

        [ForeignKey("ToWardId")]
        public Ward? ToWard { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }


        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Movement Date")]
        public DateTime MovedAt { get; set; } = DateTime.Now;

        //For Auditing
        public string? MovedByUserId { get; set; }
        [ForeignKey("MovedByUserId")]
        public ApplicationUser? MovedByUser { get; set; }


        // Navigation Property
        public Admission? Admission { get; set; }
        public virtual Patient? Patient { get; set; }

    }
}

