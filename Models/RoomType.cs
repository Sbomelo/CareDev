using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class RoomType
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        [StringLength(50)]
        public required string RoomName { get; set; } // e.g., "General", "ICU", "Private"

        public bool IsAvailable { get; set; } = true; // Indicates if the room type is currently available

        public virtual ICollection<Admission> Admissions { get; set; }// = new List<Admission>(); // Navigation property for related admissions




    }
}
