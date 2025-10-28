namespace CareDev.Models.ViewModels
{
    public class AuditDetailsViewModel
    {
        public AuditEntry AuditEntry { get; set; } = null!;
        public List<AuditFieldChange> Changes { get; set; } = new List<AuditFieldChange>();
    }
}
