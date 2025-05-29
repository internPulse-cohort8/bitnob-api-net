namespace InternPulse4.Models.UserModel
{
    public class ConfirmEmail
    {
        public string Email { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
