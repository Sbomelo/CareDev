using CareDev.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using  System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CareDev.Models
{
    public class Vital
    {
        [Key]
        public int VitalId { get; set; }

        public long PatientId { get; set; }
        public IEnumerable<SelectListItem> patients { get; set; }
        public Patient Patient { get; set; }
        public int Temperature { get; set; } 
        public int HeartRate { get; set; }
        public int BloodPressure { get; set; }
        public DateTime RecordDate { get; set; }


    }
}
