using CareDev.Models;

namespace CareDev.Heplers
{
    public class PersonalExportPackage
    {
        public Patient Patient { get; set; }
        public IEnumerable<AdmissionDto> Admissions { get; set; }
        public IEnumerable<DischargeDto> Discharges { get; set; }
        public IEnumerable<AuditEntry> Audits { get; set; }
    }
}
