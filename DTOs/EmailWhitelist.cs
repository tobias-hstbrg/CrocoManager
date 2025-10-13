using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CrocoManager.DTOs
{
    [Table("email_whitelist")]
    public class EmailWhitelist : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("email")]
        public string? email { get; set; } = string.Empty;

        [Column("role")]
        public string? role { get; set; } = string.Empty;
    }
}
