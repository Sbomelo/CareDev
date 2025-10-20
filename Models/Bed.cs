using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Models
{
    public class Bed
    {
        [Key]
        public int BedId { get; set; }

        [Required]
        public required string BedNumber{ get; set; } 

        //Foreign Key
        [Required]
        [Display(Name = "Ward Name")]
        public int WardId { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = true;

        // Navigation Properies
        [ForeignKey("WardId")]
        public virtual Ward? Ward { get; set; } 
        public virtual Admission? Admissions { get; set; } 

        [NotMapped]
        public string BedStatus => IsAvailable ? "Occupied" : "Available"; // Computed property to display bed status
    }
}
