using System.ComponentModel.DataAnnotations;
namespace CareDev.Models
{
    public class GenderOption
    {
        [Key]
        public int GenderOptionId { get; set; }
        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        public ICollection<Patient> Patients { get; set; }
    }
}
