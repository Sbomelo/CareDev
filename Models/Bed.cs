using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class Bed
    {
        [Key]
        public int BedId { get; set; }

        [Required]
        public bool Status { get; set; }

        //Foreign Key
        [Required]
        public int WardId { get; set; }
        
        // Navigation Property
        public virtual Ward Ward { get; set; } = null!; // Non-nullable reference type, must be initialized 

    }
}
