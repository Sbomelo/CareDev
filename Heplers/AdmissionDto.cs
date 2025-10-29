using CareDev.Models;

namespace CareDev.Heplers
{
    public class AdmissionDto
    {
        public int AdmissionId { get; set; }
        public DateTime AdmittedAt { get; set; }
        public DateTime? DischargedAt { get; set; }
        public string Ward { get; set; }
        public string Bed { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public int PatientId { get; set; }
        public string Reason { get; set; }


    }
}
