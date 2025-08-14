using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Models
{
    public class TreatPatient
    {
        [Key]
        public long TreatmentID{ get; set; }
        public long PatientID { get; set; }
        public Patient Patient { get; set; }
        public DateTime TreatmentDate { get; set; } 
        public string TreatmentDescription { get; set; }

        public bool IsFollowUpRequired { get; set; }

        public DateTime? FollowUpDate { get; set; } 

    }
}
