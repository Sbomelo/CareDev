using CareDev.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CareDev.Models.ApplicationUser;

namespace CareDev.Models
{
    public enum AppointmetStatus 
    {
       Pending,
       Confirmed,
       Rescheduled,
       Cancelled,
       Completed
    }

    public class Appointment
    {
        public int Id { get; set; }    // EF Core treats "Id" as PK by convention:contentReference[oaicite:0]{index=0}.
       
        public string DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public ApplicationUser? Doctor { get; set; }

        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public ApplicationUser? Patient { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }
        
        [MaxLength(1000)]
        public string? Notes { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }




    }
}
