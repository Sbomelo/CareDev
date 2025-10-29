using CareDev.Heplers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CareDev.Models.ViewModels
{
    public class DownloadFilteredAdmission
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? WardId { get; set; }
        public int? DoctorId { get; set; }
        public string Format { get; set; } = "csv";

        // UI lists
        public IEnumerable<SelectListItem> Wards { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();

        public IEnumerable<AdmissionDto> Results { get; set; } = new List<AdmissionDto>();
    }
}
