namespace CareDev.Heplers
{
    public class DischargeDto
    {
        public int DischargeId { get; set; }
        public int AdmissionId { get; set; }
        public DateTime DischargedAt { get; set; }
        public string Summary { get; set; }
        public string FollowUp { get; set; }
    }
}
