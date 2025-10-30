using System.ComponentModel.DataAnnotations;

namespace CareDev.Models.ViewModels
{
    public class PatientCreateViewModel
    {
        public string? Id { get; set; }

        [Required, StringLength(30)]
        public string Name { get; set; }

        [Required, StringLength(30)]
        public string SurName { get; set; }
        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required, Range(1, 100)]
        public int Age { get; set; }

        [Required]
        [Display(Name = "Address")]
        [StringLength(250)]
        public string? Address { get; set; }

        [Required]
        [StringLength(50)]
        public string? City { get; set; }

        [Required]
        [Display(Name = "ID Number")]
        [StringLength(13, ErrorMessage = "ID Number Must be 13 digits")]
        public string IDNumber { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; }

        [Required, RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits starting with 0")]
        public string PhoneNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}

