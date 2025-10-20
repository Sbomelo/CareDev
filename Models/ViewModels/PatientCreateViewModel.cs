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

        [Required, Range(1, 100)]
        public int Age { get; set; }

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
