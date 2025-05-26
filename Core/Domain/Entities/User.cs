using InternPulse4.Core.Domain.Enums;
using System.Text.Json.Serialization;

namespace InternPulse4.Core.Domain.Entities
{
    public class User : Auditables
    {
        [JsonInclude]
        public string FirstName { get; set; }
        [JsonInclude]
        public string LastName { get; set; }
        [JsonInclude]
        public string Email { get; set; }
        [JsonInclude]
        public string? Password { get; set; }
        [JsonInclude]
        public Role Role { get; set; }
    }
}
