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
        [StringLength(30, ErrorMessage = "The Name Cannnot Be Longer than 30 Characters")]
        public string Name { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The Name Cannnot Be Longer than 30 Characters")]
        public string SurName { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Age Must Be Between 1 AND 100 Years Old")]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public bool Active { get; set; } = true;


        //Relationships 
        public virtual ICollection<Admission> Admissions { get; set; } = new HashSet<Admission>();
        public virtual ICollection<TreatPatient> TreatPatients { get; set; } = new HashSet<TreatPatient>();
        public virtual ICollection<Medication> Medications { get; set; } = new HashSet<Medication>();


    }
}
