using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class AuditEntry
    {
        [Key]
        public int AuditEntryId { get; set; }

        [Required]
        public string TableName { get; set; } = null!;           // "Patients" or "Employees"

        [Required]
        public string Action { get; set; } = null!;              // "Insert", "Update", "Delete"

        // Primary key values as JSON (simple dictionary)
        public string? KeyValues { get; set; }

        // Changed columns (comma-separated)
        public string? ChangedColumns { get; set; }

        // Old values as JSON
        public string? OldValues { get; set; }

        // New values as JSON
        public string? NewValues { get; set; }

        // User who made the change (ApplicationUser.Id)
        public string? UserId { get; set; }

        // Optional: human-readable username (if you prefer storing it)
        public string? UserName { get; set; }

        // timestamp (UTC)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

