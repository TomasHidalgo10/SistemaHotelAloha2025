using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using System.Xml.Linq;

namespace SistemaHotelAloha.Web.Auth
{
    public sealed class SimpleAuthStateProvider : AuthenticationStateProvider
    {
        private const string StorageKey = "auth_session_v1";
        private readonly ProtectedSessionStorage _storage;

        public SimpleAuthStateProvider(ProtectedSessionStorage storage)
        {
            _storage = storage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var result = await _storage.GetAsync<AuthSession>(StorageKey);
                var session = result.Success ? result.Value : null;
                if (session is null)
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

                var identity = BuildIdentity(session);
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task SignInAsync(int userId, string name, string email, string role = "User")
        {
            var session = new AuthSession
            {
                UserId = userId,
                Name = string.IsNullOrWhiteSpace(name) ? email : name,
                Email = email ?? "",
                Role = string.IsNullOrWhiteSpace(role) ? "User" : role
            };

            await _storage.SetAsync(StorageKey, session);

            var identity = BuildIdentity(session);
            var authState = new AuthenticationState(new ClaimsPrincipal(identity));
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task SignOutAsync()
        {
            try { await _storage.DeleteAsync(StorageKey); } catch { }
            var anon = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            NotifyAuthenticationStateChanged(Task.FromResult(anon));
        }

        private static ClaimsIdentity BuildIdentity(AuthSession s)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, s.UserId.ToString()),
                new(ClaimTypes.Name, s.Name ?? s.Email ?? "Usuario"),
                new(ClaimTypes.Email, s.Email ?? ""),
                new(ClaimTypes.Role, s.Role ?? "User")
            };
            return new ClaimsIdentity(claims, authenticationType: "SimpleAuth");
        }

        private sealed class AuthSession
        {
            public int UserId { get; set; }
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? Role { get; set; }
        }
    }
}
