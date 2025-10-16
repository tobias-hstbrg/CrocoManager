using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CrocoManager.DTOs
{
    public class UpdatedUserRoleReponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("user")]
        public UserDto User { get; set; }

        public class UserDto
        {
            [JsonPropertyName("id")]
            public Guid Id { get; set; }
            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("role")]
            public string Role { get; set; }
        }
    }
}
