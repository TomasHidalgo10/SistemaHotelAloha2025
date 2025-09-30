using Microsoft.AspNetCore.Components.Authorization;
using SistemaHotelAloha.Web.Auth;
using SistemaHotelAloha.Web.Data;
using SistemaHotelAloha.Web.Models;
using SistemaHotelAloha.Web.Security;

namespace SistemaHotelAloha.Web.Services;

public class AuthService
{
    private readonly UserRepository _repo;
    private readonly PasswordHasher _hasher;
    private readonly SimpleAuthStateProvider _auth;

    public AuthService(UserRepository repo, PasswordHasher hasher, AuthenticationStateProvider authProvider)
    {
        _repo = repo;
        _hasher = hasher;
        _auth = (SimpleAuthStateProvider)authProvider;
    }

    public async Task<(bool ok, string? error)> RegisterAsync(string userName, string? email, string password)
    {
        var exists = await _repo.GetByUserNameAsync(userName);
        if (exists != null) return (false, "El usuario ya existe");
        var (hash, salt) = _hasher.HashPassword(password);
        var user = new User { UserName = userName, Email = email, PasswordHash = hash, Salt = salt };
        var id = await _repo.CreateAsync(user);
        if (id <= 0) return (false, "No se pudo crear el usuario");
        _auth.SignIn(userName);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> LoginAsync(string userName, string password)
    {
        var u = await _repo.GetByUserNameAsync(userName);
        if (u == null) return (false, "Usuario o contraseña inválidos");
        var ok = _hasher.Verify(password, u.Salt, u.PasswordHash);
        if (!ok) return (false, "Usuario o contraseña inválidos");
        _auth.SignIn(u.UserName);
        return (true, null);
    }

    public void Logout() => _auth.SignOut();
}
