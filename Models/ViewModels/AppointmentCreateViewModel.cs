using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CareDev.Models.ViewModels
{
    public class AppointmentCreateViewModel
    {
        [Required]
        public string DoctorId { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime Start { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTime End { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem>? Doctors { get; set; }
    }
}
