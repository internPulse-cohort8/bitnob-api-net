namespace InternPulse4.Core.Domain.Entities
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public string TokenHash { get; set; } 
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
