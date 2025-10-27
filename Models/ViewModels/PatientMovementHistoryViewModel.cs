namespace CareDev.Models.ViewModels
{
    public class PatientMovementHistoryViewModel
    {
        public int PatientId { get; set; }
        public string PatientFullName { get; set; } = string.Empty;

        // optional filter: admission id (null = show all)
        public int? AdmissionId { get; set; }

        // date-range filter
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        // text search filter
        public string? Query { get; set; }


        // list of admission options so patient can filter by a specific admission
        public List<Admission> Admissions { get; set; } = new List<Admission>();

        // the movements to display
        public List<PatientMovement> Movements { get; set; } = new List<PatientMovement>();
    }
}
