using CareDev.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CareDev.Models.ApplicationUser;

namespace CareDev.Models
{
    public class Appointment
    {
        public int Id { get; set; }    // EF Core treats "Id" as PK by convention:contentReference[oaicite:0]{index=0}.
        public string DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public ApplicationUser? Doctor { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public ApplicationUser? Patient { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public AppointmentStatus? Status { get; set; } = AppointmentStatus.Pending;
        public string Notes { get; set; }
    }
}
