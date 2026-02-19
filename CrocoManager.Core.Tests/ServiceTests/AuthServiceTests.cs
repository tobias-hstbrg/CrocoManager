using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using CrocoManager.Core.Services;
using FluentAssertions;
using Supabase;

public class AuthServiceTests
{
    class DummySupabaseClientService : ISupabaseClientService
    {
        public Client Client => throw new NotImplementedException(); // never accessed in ParseUserRole
        public Task InitializeAsync() => Task.CompletedTask;
    }

    class DummySecureStorageService : ISecureStorageService
    {
        public Task SetAsync(string key, string value) => Task.CompletedTask;
        public Task<string?> GetAsync(string key) => Task.FromResult<string?>(null);
        public Task Remove(string key) => Task.CompletedTask;
    }

    [Theory]
    [InlineData("NotAssigned", UserRole.NotAssigned)]
    [InlineData("Scientist", UserRole.Scientist)]
    [InlineData("Admin", UserRole.Admin)]
    [InlineData("Ranger", UserRole.Ranger)]
    public void ParseUserRole_Should_ParseCorrectly(string input, UserRole expected)
    {
        var service = new SupabaseAuthService(
            new DummySupabaseClientService(),
            new DummySecureStorageService()
        );

        // Use reflection to invoke the private method
        var role = (UserRole)service.GetType().GetMethod("ParseUserRole", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .Invoke(service, new object?[] { input })!;

        role.Should().Be(expected);
    }

    [Fact]
    public void BuildSession_Should_MapUserMetadataToCorrectRole()
    {
        // Arrange
        var service = new SupabaseAuthService(new DummySupabaseClientService(), new DummySecureStorageService());

        // Wir bauen ein Fake-Objekt, das so aussieht wie die Antwort von Supabase
        var fakeAuthResponse = new Supabase.Gotrue.Session
        {
            AccessToken = "abc-token",
            RefreshToken = "def-refresh",
            TokenType = "bearer",
            ExpiresIn = 3600,
            User = new Supabase.Gotrue.User
            {
                Id = "user123-uuid",
                Email = "test@croco.com",
                UserMetadata = new Dictionary<string, object> { { "role", "Scientist" } }
            }
        };

        // Act: Private Methode aufrufen
        var method = service.GetType().GetMethod("BuildSession", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var session = (SupabaseSession)method!.Invoke(service, new object[] { fakeAuthResponse })!;

        // Assert
        session.User.UserMetadata.Role.Should().Be(UserRole.Scientist);
        session.AccessToken.Should().Be("abc-token");
        session.RefreshToken.Should().Be("def-refresh");
        session.User.Email.Should().Be("test@croco.com");
    }
}
