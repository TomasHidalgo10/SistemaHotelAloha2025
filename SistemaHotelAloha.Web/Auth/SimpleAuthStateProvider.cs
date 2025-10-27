using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using System.Xml.Linq;

namespace SistemaHotelAloha.Web.Auth
{
    public class SimpleAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _storage;
        private readonly NavigationManager _nav;

        public SimpleAuthStateProvider(ProtectedSessionStorage storage, NavigationManager nav)
        {
            _storage = storage;
            _nav = nav;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Si estás con prerender desactivado (render-mode="Server"), no hará falta este guard.
                // Lo dejo por seguridad; si hay prerender, devolvemos usuario no autenticado.
                if (_nav.Uri is null)
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

                var result = await _storage.GetAsync<SessionUser>("auth");
                var userSession = result.Success ? result.Value : null;

                if (userSession is null || string.IsNullOrWhiteSpace(userSession.Username))
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.Username)
                };

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "auth"));
                return new AuthenticationState(principal);
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        // ✅ Métodos que tus páginas ya intentan usar
        public async Task SignInAsync(string username)
        {
            var session = new SessionUser { Username = username ?? string.Empty };
            await _storage.SetAsync("auth", session);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task SignInAsync(string username, string password)
        {
            // Podés almacenar solo el username; la contraseña no se guarda por seguridad
            var session = new SessionUser { Username = username ?? string.Empty };
            await _storage.SetAsync("auth", session);

            // Notifica a Blazor que el usuario cambió
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        public async Task SignOutAsync()
        {
            await _storage.DeleteAsync("auth");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        // (opcional) si en algún lado llamás este nombre
        public Task UpdateAuthenticationState(SessionUser? session) =>
            session is null ? SignOutAsync() : SignInAsync(session.Username);
    }

    public class SessionUser
    {
        public string Username { get; set; } = string.Empty;
    }
}