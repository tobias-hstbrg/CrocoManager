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
        public UserMetadata? UserMetadata { get; set; } = new UserMetadata();
        public DateTime? CreatedAt { get; set; }
    }

    public class UserMetadata
    {
        public UserRole? Role { get; set; } = UserRole.NotAssigned;
    }

    public enum UserRole
    {
        Admin,
        Ranger,
        Scientist,
        NotAssigned
    }
}
