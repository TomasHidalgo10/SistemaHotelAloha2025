using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SistemaHotelAloha.Web.Auth
{
    public class SimpleAuthStateProvider : AuthenticationStateProvider
    {
        private const string StorageKey = "auth_user";
        private readonly ProtectedSessionStorage _session;
        private readonly string _connString;

        public SimpleAuthStateProvider(ProtectedSessionStorage session, IConfiguration cfg)
        {
            _session = session;
            _connString = cfg.GetConnectionString("DefaultConnection")
                           ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection");
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var res = await _session.GetAsync<AuthUser>(StorageKey);
                if (res.Success && res.Value is not null)
                    return new AuthenticationState(BuildPrincipal(res.Value));
            }
            catch { /* anónimo si falla session */ }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return false;

            email = email.Trim();

            // 1) Traemos el usuario SOLO por email (case-insensitive)
            AuthUser? data = null;
            string? passDb = null;

            await using (var cn = new MySqlConnection(_connString))
            {
                await cn.OpenAsync();
                const string sql = @"
                    SELECT Id, Nombre, Apellido, Email, Contrasena
                    FROM usuarios
                    WHERE LOWER(TRIM(Email)) = LOWER(TRIM(@e))
                    LIMIT 1;";
                await using var cmd = new MySqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@e", email);
                await using var rd = await cmd.ExecuteReaderAsync();

                if (await rd.ReadAsync())
                {
                    data = new AuthUser
                    {
                        Id = rd.GetInt32("Id"),
                        Nombre = rd.GetString("Nombre"),
                        Apellido = rd.GetString("Apellido"),
                        Email = rd.GetString("Email")
                    };
                    passDb = rd["Contrasena"]?.ToString();
                }
            }

            if (data is null || string.IsNullOrEmpty(passDb))
                return false;

            // 2) Normalizamos y comparamos la contraseña
            var db = passDb.Trim();

            // a) si la DB guarda texto plano
            if (db == password)
            {
                await PersistAndSignIn(data);
                return true;
            }

            // b) si la DB guarda SHA-256 en HEX (64 chars)
            var passSha = Sha256Hex(password);               // minúsculas
            if (db.Length == 64 && db.Equals(passSha, StringComparison.OrdinalIgnoreCase))
            {
                await PersistAndSignIn(data);
                return true;
            }

            // c) (opcional) si algún día guardás bcrypt (comienza con $2a$/$2b$)
            //    podrías usar BCrypt.Net-Next; aquí no lo incluimos para no agregar NuGet.

            return false;
        }

        private async Task PersistAndSignIn(AuthUser data)
        {
            await _session.SetAsync(StorageKey, data);
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(BuildPrincipal(data)))
            );
        }

        public async Task LogoutAsync()
        {
            await _session.DeleteAsync(StorageKey);
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
            );
        }

        public Task SignOutAsync() => LogoutAsync();

        private static ClaimsPrincipal BuildPrincipal(AuthUser u)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, u.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{u.Nombre} {u.Apellido}"),
                new Claim(ClaimTypes.Email, u.Email)
            };
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "simple"));
        }

        private static string Sha256Hex(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString(); // minúsculas
        }

        // Público para (de)serialización
        public class AuthUser
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = "";
            public string Apellido { get; set; } = "";
            public string Email { get; set; } = "";
        }
    }
}
