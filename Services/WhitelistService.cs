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
            var user = await _supabase.Client.From<EmailWhitelist>().Where(entry => entry.Id == id).Single();

            // check if user exists on whitelist if not return false;
            if (user == null || user.Email == null)
                return false;

            // Try updating user in the supabase auth list when an account for that email is available
            var userUpdated = await UpdateAuthUserRoleIfExists(user.Email, newRole);

            // Even if there is not an authed user with that email. Update Role on whitelist
            // This is because an admin wants to update a user role for an account that has yet to sign up.
            var response = await _supabase.Client
                                .From<EmailWhitelist>()
                                .Where(entry => entry.Id == id)
                                .Set(entry => entry.Role, newRole.ToString())
                                .Update();

            // Check if the update on the whitelist table was successful or if the update on a possible authed user was successful
            if (response?.Models == null || response.Models.Count == 0 || !userUpdated)
            {
                //throw new InvalidOperationException($"Failed to update role for whitelist entry with id {id}. No rows were affected.");
                return false;
            }
            return true;
        }

        public async Task DeleteEmailFromWhitelistAsync(Guid id)
        {
            await _supabase.Client.From<EmailWhitelist>().Where(entry => entry.Id == id).Delete();
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
