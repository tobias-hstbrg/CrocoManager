using CrocoManager.DTOs;
using CrocoManager.Interfaces;
using CrocoManager.Models;
using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Supabase.Functions.Client;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

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

        public async Task<bool> UpdateRoleAsync(Guid id, UserRole newRole)
        {
            var user = await _supabase.Client
                .From<EmailWhitelist>()
                .Where(entry => entry.Id == id)
                .Single();

            // Check if user exists on whitelist, if not return false
            if (user?.Email == null)
                return false;

            // Try updating user in the supabase auth list when an account for that email is available
            // This is optional - there might not be an auth user for that email yet.
            var userUpdated = await UpdateAuthUserRoleIfExists(user.Email, newRole);

            // Update role on whitelist
            var response = await _supabase.Client
                .From<EmailWhitelist>()
                .Where(entry => entry.Id == id)
                .Set(entry => entry.Role!, newRole.ToString())
                .Update();

            // Check if whitelist update succeeded
            var whitelistUpdated = response?.Models != null && response.Models.Count > 0;

            // Return true if whitelist was updated
            return whitelistUpdated || userUpdated;
        }

        public async Task<bool> DeleteEmailFromWhitelistAsync(Guid id, string email)
        {
            var options = new InvokeFunctionOptions
            {
                Body = new Dictionary<string, object>
        {
            { "id", id },
            { "email", email }
        }
            };

            var response = await _supabase.Client.Functions.Invoke("delete-user", null, options);

            if (string.IsNullOrWhiteSpace(response))
                return false;

            // Parse the JSON
            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            // Extract the success property
            if (root.TryGetProperty("success", out var successProp) && successProp.ValueKind == JsonValueKind.True)
                return true;

            return false;
        }

        private async Task<bool> UpdateAuthUserRoleIfExists(string email, UserRole newRole)
        {
            try
            {
                var options = new Supabase.Functions.Client.InvokeFunctionOptions
                {
                    Body = new Dictionary<string, object>
                {
                    { "email", email },
                    { "role",  newRole.ToString() }
                }
                };
                var response = await _supabase.Client.Functions.Invoke("updateUserRoleIfUserExists", options: options);

                var result = JsonSerializer.Deserialize<UpdatedUserRoleReponse>(response);

                if (result == null)
                    throw new InvalidOperationException($"Invalid response from function: {response}");

                return result.Success;
            }
            catch
            {
                return false;
            }
        }
    }
}
