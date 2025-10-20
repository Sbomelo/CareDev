using Microsoft.CodeAnalysis.Diagnostics;
using System.ComponentModel.DataAnnotations;


namespace CareDev.Models.ViewModels
{
    public class DischargeViewModel
    {
        public int AdmissionId { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Patient Name")]
        public string? PatientName { get; set; }

        public int? BedId { get; set; }

        [Display(Name = "Bed Number")]
        public string? BedNumber { get; set; }

        [Display(Name ="Admission Date")]
        public DateTime AdmissionDate { get; set; }

        [Display(Name = "Discharge Date")]
        public DateTime? DischargeDate { get; set; }

        [Display(Name = "Discharge Notes")]
        [StringLength(500, ErrorMessage = "Discharge notes cannot exceed 500 characters.")]
        [DataType(DataType.MultilineText)]
        public string? DischargeNotes { get; set; }

    }
}
