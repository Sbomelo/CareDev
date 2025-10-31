namespace CareDev.Models
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }          // FK to AspNetUsers.Id
        public string PasswordHash { get; set; }    // the hashed password
        public DateTime CreatedAt { get; set; }
    }
}
