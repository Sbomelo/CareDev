using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Models
{
    public class Bed
    {
        [Key]
        public int BedId { get; set; }

        [Required]
        public bool Status { get; set; } = true;

        //Foreign Key
        [Required]
        [Display(Name = "Ward Name")]
        public int WardId { get; set; }
        //public int? AdmissionId { get; set; }

        // Navigation Properies
        [ForeignKey("WardId")]
        public virtual Ward Ward { get; set; } 
        public virtual Admission? Admissions { get; set; } 

        [NotMapped]
        public string BedStatus => Status ? "Available" : "Occupied";
    }
}
