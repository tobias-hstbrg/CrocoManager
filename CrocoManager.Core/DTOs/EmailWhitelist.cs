using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CrocoManager.Core.DTOs
{
    [Table("email_whitelist")]
    public class EmailWhitelist : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("email")]
        public string? Email { get; set; } = string.Empty;

        [Column("role")]
        public string? Role { get; set; } = string.Empty;
    }

    public sealed class WhitelistResponse
    {
        [JsonPropertyName("whitelisted")]
        public bool Whitelisted { get; set; }

        [JsonPropertyName("role")]
        public string? Role { get; set; }
    }
}
