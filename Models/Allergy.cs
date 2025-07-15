
using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class Allergy
    {
        [Key]
        public int AllergyId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

       public ICollection<Patient> Patients { get; set; } = new List<Patient>();


    }
}
