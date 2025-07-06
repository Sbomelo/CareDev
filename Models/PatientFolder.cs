using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class PatientFolder
    {
        [Key]
        public int PatientFolderId { get; set; }

        [Required]
        [StringLength(100)]
        public required string FolderName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign Key
        [Required]
        public int PatientId { get; set; }

        // Navigation Property
        public virtual Patient Patient { get; set; } = null!; // Non-nullable reference type, must be initialized 
    }
}
