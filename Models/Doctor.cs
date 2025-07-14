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
        [EmailAddress(ErrorMessage ="Please Enter a valid email address.")]
        [Display(Name = "Email Address", Prompt ="example.@gmail.com")]
        public required string Email { get; set; } 

        [Required]
        [StringLength(50)]
        public required string Specialization { get; set; } 


        // Navigation Properties
        public virtual ICollection<Admission> Admissions { get; set; } //= new List<Admission>();
    }
}
