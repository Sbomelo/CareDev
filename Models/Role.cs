using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Permissions { get; set; }

        // Navigation Property
        public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
    }
}
