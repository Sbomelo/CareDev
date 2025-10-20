using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        [StringLength(50)]
        public required string SurName { get; set; }

        [Required]
        [Range(24, 65, ErrorMessage = "Age Must Be Between 24 AND 65 Years Old")]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress(ErrorMessage ="Please Enter a valid email address.")]
        [Display(Name = "Email Address", Prompt ="example.@gmail.com")]
        public required string Email { get; set; } 

        [Required]
        [StringLength(50)]
        public required string Specialization { get; set; } 

        // Navigation Properties
        public virtual ICollection<Admission>? Admissions { get; set; } = new List<Admission>();
    }
}
