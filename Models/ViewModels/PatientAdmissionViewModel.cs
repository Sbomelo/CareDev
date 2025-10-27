namespace CareDev.Models.ViewModels
{
    public class PatientAdmissionViewModel
    {
        public int PatientId { get; set; }
        public string PatientFullName { get; set; } = string.Empty;

        // active admission (null if none)
        public Admission? CurrentAdmission { get; set; }

        // movements for the active admission (or specific admission)
        public List<PatientMovement> Movements { get; set; } = new List<PatientMovement>();

        // recent admissions (history) to show discharged summaries
        public List<Admission> RecentAdmissions { get; set; } = new List<Admission>();
    }
}
