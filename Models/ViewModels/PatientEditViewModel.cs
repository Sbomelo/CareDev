using System.ComponentModel.DataAnnotations;

namespace CareDev.Models.ViewModels
{
    public class PatientEditViewModel
    {
        public string Id { get; set; } // Needed to identify the patient

        [Required, StringLength(30)]
        public string Name { get; set; }

        [Required, StringLength(30)]
        public string SurName { get; set; }

        [Required, Range(1, 100)]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required, RegularExpression(@"^0\d{9}$")]
        public string PhoneNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

       
        
    }
}
