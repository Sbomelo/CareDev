using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

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
    public string Notes { get; set; }

    // optional: list of doctors for select list
    public IEnumerable<SelectListItem> Doctors { get; set; }
  }


