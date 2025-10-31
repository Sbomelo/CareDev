using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Models
{
    public class DoctorAvailability
    {
        public int Id { get; set; }

        [Required]
        public string DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public ApplicationUser Doctor { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; } 
        
        [Required]
        public TimeSpan EndTime { get; set; }   

        public bool IsAvailable { get; set; } = true;  // false if on leave, etc.
    }
}
