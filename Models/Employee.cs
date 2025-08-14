using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } 

        [Required]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Specialization { get; set; }

        [Required]
        public bool Active { get; set; } = true;

        //Navigation Properties
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        //Relationships 
        public virtual ICollection<Admission> Admissions { get; set; }
        public virtual ICollection<TreatPatient> TreatPatients { get; set; }
        public virtual ICollection<Medication> Medications { get; set; } 
       

    }
}
