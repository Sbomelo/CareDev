using Microsoft.AspNetCore.Mvc.Rendering;

namespace CareDev.Models.ViewModels
{
    public class DownloadOptionsViewModel
    {
        // Selected options (bound from the form)
        public string Format { get; set; } = "json";          // json, csv, pdf, zip
        public bool IncludeAudits { get; set; } = false;

        // Optional filters
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? WardId { get; set; }
        public int? DoctorId { get; set; }

        // For the UI select lists
        public IEnumerable<SelectListItem> AvailableFormats { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Wards { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();

        // Optional: additional UI text/help
        public string Note { get; set; } = "Choose the format and filters, then click Download.";

        public DownloadOptionsViewModel()
        {
            // sensible defaults for formats (can be overridden by controller)
            AvailableFormats = new[]
            {
                new SelectListItem("JSON", "json"),
                new SelectListItem("CSV (zipped)", "csv"),
                new SelectListItem("PDF", "pdf", selected: true),
                new SelectListItem("ZIP (separate files)", "zip")
            };
        }
    }
}