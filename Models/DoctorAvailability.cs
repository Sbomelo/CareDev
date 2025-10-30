using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Models
{
    public class DoctorAvailability
    {
        public int Id { get; set; }
        public string DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public ApplicationUser Doctor { get; set; }
        public DayOfWeek DayOfWeek { get; set; }  
        public TimeSpan StartTime { get; set; }    
        public TimeSpan EndTime { get; set; }   
        public bool IsAvailable { get; set; } = true;  // false if on leave, etc.
    }
}
