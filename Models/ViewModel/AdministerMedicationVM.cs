using System.ComponentModel.DataAnnotations;

public class AdministerMedicationVM
{
    public int DispenseId { get; set; }
    public string MedicationName { get; set; }
    public DateTime TimeGiven { get; set; } = DateTime.Now;

    [MaxLength(500)]
    public string Observations { get; set; }

    [MaxLength(250)]
    public string AdverseReactions { get; set; }
}
