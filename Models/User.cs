using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Models
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }

    [Table("test")]
    public class Test : BaseModel
    {
        [PrimaryKey("id")]
        public int id { get; set; }

        [Column("text")]
        public string text { get; set; } = string.Empty;
    }
}
