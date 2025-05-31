using System.ComponentModel.DataAnnotations;

namespace InternPulse4.Models.UserModel
{
    public class ForgotPasswordRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
