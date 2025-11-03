using SistemaHotelAloha.AccesoDatos;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.Web.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _local;
    private readonly UsuarioAdoRepository _repo;
    private const string Key = "auth_v1"; // clave de storage

    public CustomAuthStateProvider(ProtectedLocalStorage local, UsuarioAdoRepository repo)
    {
        _local = local;
        _repo = repo;
    }

    public static string Hash(string raw)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw ?? string.Empty));
        var sb = new StringBuilder();
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    public async Task<bool> SignInAsync(string email, string password)
    {
        var id = _repo.Authenticate(email.Trim(), Hash(password));
        if (id <= 0) return false;

        var fullName = _repo.GetNombreCompletoById(id);
        await _local.SetAsync(Key, new AuthDto { Id = id, Email = email.Trim(), Name = fullName });

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return true;
    }

    public async Task SignOutAsync()
    {
        await _local.DeleteAsync(Key);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var stored = await _local.GetAsync<AuthDto>(Key);
            if (stored.Success && stored.Value is { } a && a.Id > 0)
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, a.Id.ToString()),
                    new Claim(ClaimTypes.Name, a.Name ?? a.Email ?? "Usuario"),
                    new Claim(ClaimTypes.Email, a.Email ?? "")
                }, "Local");

                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
        }
        catch { /* ignore */ }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    private class AuthDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
}