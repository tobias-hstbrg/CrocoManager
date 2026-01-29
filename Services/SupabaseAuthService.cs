using CrocoManager.DTOs;
using CrocoManager.Interfaces;
using CrocoManager.Models;
using Microsoft.IdentityModel.Tokens;
using Supabase;
using Supabase.Interfaces;
using Supabase.Postgrest.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Supabase.Functions.Client;

namespace CrocoManager.Services
{
    public sealed class SupabaseAuthService : IAuthService
    {
        private readonly SupabaseClientService _supabase;

        public SupabaseAuthService(SupabaseClientService supabase)
        {
            _supabase = supabase;
        }
        public async Task InitializeAsync()
        {
            await _supabase.InitializeAsync();
        }


        public async Task<SupabaseSession?> RegisterAsync(string email, string password)
        {
            try
            {
                var whitelistResponse = await CheckEmailWhitelist(email);
                if (whitelistResponse == null)
                    return null;

                // grab Userrole in usable format
                var newUsersRole = ParseUserRole(whitelistResponse.Role);

                // Prepare user_metadata for new user
                var options = new Supabase.Gotrue.SignUpOptions
                {
                    Data = new Dictionary<string, object>
                    {
                        { "role", newUsersRole.ToString() }
                    }
                };

                // retrieves a supabase session and the new user (hopefully)
                var authResponse = await _supabase.Client.Auth.SignUp(email, password, options);
                if (authResponse?.User == null)
                    return null;

                var session = BuildSession(authResponse);

                // safe session in secure storage of the operting system
                var sessionJson = JsonSerializer.Serialize(session);
                await SecureStorage.SetAsync("supabase_session", sessionJson);

                return session;
            }
            catch (PostgrestException ex)
            {
                Console.WriteLine($"Supabase error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error {ex.Message}");
            }
            return null;
        }

        private async Task<EmailWhitelist?> CheckEmailWhitelist(string email)
        {
            var options = new InvokeFunctionOptions
            {
                Body = new Dictionary<string, object>
                {
                    { "email", email }
                }
            };

            var response = await _supabase.Client.Functions.Invoke("check-email-whitelist", null, options);

            if(string.IsNullOrWhiteSpace(response)) return null;


            var result = JsonSerializer.Deserialize<WhitelistResponse>(response);

            if(result == null || !result.Whitelisted) return null;

            return new EmailWhitelist
            {
                Email = email,
                Role = result.Role
            };
        }

        private SupabaseSession BuildSession(Supabase.Gotrue.Session authResponse)
        {
            if (authResponse?.User == null)
                throw new InvalidOperationException("Invalid auth response: User data is missing");

            if (string.IsNullOrEmpty(authResponse.AccessToken))
                throw new InvalidOperationException("Invalid auth response: Access token is missing");

            if (string.IsNullOrEmpty(authResponse.RefreshToken))
                throw new InvalidOperationException("Invalid auth response: Refresh token is missing");

            if (string.IsNullOrEmpty(authResponse.TokenType))
                throw new InvalidOperationException("Invalid auth response: Token type is missing");

            if(string.IsNullOrEmpty(authResponse.User.Id))
                throw new InvalidOperationException("Invalid auth response: User ID is missing");

            if(string.IsNullOrEmpty(authResponse.User.Email))
                throw new InvalidOperationException("Invalid auth response: User Email is missing");

            var userMetadata = authResponse.User.UserMetadata;
            var role = ParseUserRole(userMetadata?["role"]?.ToString());

            return new SupabaseSession
            {
                AccessToken = authResponse.AccessToken,
                RefreshToken = authResponse.RefreshToken,
                TokenType = authResponse.TokenType,
                ExpiresIn = DateTime.UtcNow.AddSeconds(authResponse.ExpiresIn),
                User = new Models.User
                {
                    Id = authResponse.User.Id,
                    Email = authResponse.User.Email,
                    CreatedAt = authResponse.User.CreatedAt,
                    UserMetadata = new Models.UserMetadata
                    {
                        Role = role
                    }
                }
            };
        }

        private UserRole ParseUserRole(string? role)
        {
            if (string.IsNullOrEmpty(role))
                return UserRole.NotAssigned;

            // Tries to parse string into enum if that doesnt work it defaults to not Assigned just like if the string would be null.
            return Enum.TryParse(role, true, out UserRole parsedRole) ? parsedRole : UserRole.NotAssigned;
        }


        public async Task<SupabaseSession?> LoginAsync(string email, string password)
        {
            try
            {
                var authResponse = await _supabase.Client.Auth.SignInWithPassword(email, password);

                if (authResponse?.User == null)
                    return null;

                var session = BuildSession(authResponse);

                // session validation before setting it.
                if(string.IsNullOrEmpty(authResponse.AccessToken) || string.IsNullOrEmpty(authResponse.RefreshToken))
                {
                    throw new InvalidOperationException("Login failed: AccessToken or RefreshToken is null or empty.");
                }
                // setting session on the supabase client instance
                await _supabase.Client.Auth.SetSession(authResponse.AccessToken, authResponse.RefreshToken);

                // Save to secure storage
                var sessionJson = JsonSerializer.Serialize(session);
                await SecureStorage.SetAsync("supabase_session", sessionJson);

                return session;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SignOutAsync()
        {
            try
            {
                await _supabase.Client.Auth.SignOut();
                SecureStorage.Remove("supabase_session");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string password)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "user_email", email },
                    { "new_password", password }
                };

                // Rpc gibt BaseResponse zurück
                var response = await _supabase.Client
                    .Rpc("unsafe_reset_password", parameters);

                // Überprüfe Response Content
                if (!string.IsNullOrEmpty(response.Content))
                {
                    var result = JsonSerializer.Deserialize<ResetPasswordResult>(response.Content);
                    return result?.Success ?? false;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Reset password error: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var result = await _supabase.Client.Auth.RetrieveSessionAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    public class ResetPasswordResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;
    }
}
