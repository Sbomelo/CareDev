using System.ComponentModel.DataAnnotations;

namespace CareDev.Models.ViewModels
{
      public class MovementCreateViewModel
      {
         public int AdmissionId { get; set; }
          public int PatientId { get; set; }

         [Display(Name = "Patient")]
          public string? PatientName { get; set; }

         [Display(Name = "Current Ward")]
         public int? FromWardId { get; set; }
         [Display(Name = "Current Bed")]
         public int? FromBedId { get; set; }
         public string? FromWardName { get; set; }
         public string? FromBedNumber { get; set; }

         [Required]
         [Display(Name = "To Ward")]
          public int ToWardId { get; set; }

        [Required]
        [Display(Name = "To Bed")]
         public int ToBedId { get; set; }

        [Display(Name = "Move time")]
        public DateTime? MovedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Reason")]
        [StringLength(500)]
        public string? Reason { get; set; }
    }

}



