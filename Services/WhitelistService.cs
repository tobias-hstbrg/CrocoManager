using CrocoManager.DTOs;
using CrocoManager.Interfaces;
using CrocoManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Services
{
    public sealed class WhitelistService : IWhitelistService
    {
        SupabaseClientService _supabase;

        public WhitelistService(SupabaseClientService supabase)
        {
            _supabase = supabase;
        }

        public async Task<List<EmailWhitelist>> GetWhitelistedEmailsAsync()
        {
            var result = await _supabase.Client.From<EmailWhitelist>().Get();
            return result.Models;
        }

        public async Task AddEmailToWhitelistAsync(string email, UserRole role)
        {
            var newEntry = new EmailWhitelist
            {
                Id = Guid.NewGuid(),
                Email = email,
                Role = role.ToString()
            };
            await _supabase.Client.From<EmailWhitelist>().Insert(newEntry);
        }

        public async Task UpdateRoleAsync(Guid id, UserRole newRole)
        {
            var updateEntry = new EmailWhitelist
            {
                Role = newRole.ToString()
            };
            await _supabase.Client.From<EmailWhitelist>().Where(entry => entry.Id == id).Update(updateEntry);
        }

        public async Task DeleteEmailFromWhitelistAsync(Guid id)
        {
            await _supabase.Client.From<EmailWhitelist>().Where(entry => entry.Id == id).Delete();
        }
    }
}
