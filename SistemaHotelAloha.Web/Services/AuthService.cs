using Microsoft.AspNetCore.Components.Authorization;
using SistemaHotelAloha.Web.Auth;
using SistemaHotelAloha.Web.Data;
using SistemaHotelAloha.Web.Security;
using SistemaHotelAloha.Dominio;

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

    // Registro
    public async Task<(bool ok, string? error)> RegisterAsync(string userName, string? email, string password)
    {
        // ¿ya existe?
        var exists = await _repo.GetByUserNameAsync(userName);
        if (exists != null) return (false, "El usuario ya existe");

        // hash de la contraseña (si tu hasher devuelve (hash, salt), ignoramos el salt)
        var (hashBytes, saltBytes) = _hasher.HashPassword(password);

        // 2) Convertimos cada parte a Base64 y las unimos con ':'
        var hashBase64 = Convert.ToBase64String(hashBytes);
        var saltBase64 = Convert.ToBase64String(saltBytes);
        var contraseñaGuardada = $"{hashBase64}:{saltBase64}";  // <-- formato "hash:salt"

        // 3) Creamos el usuario con ese valor en Contraseña
        var usuario = new Usuario(
            id: 0,
            nombre: userName,
            apellido: "",
            email: email ?? userName,     // si no viene email, usamos userName
            contraseña: contraseñaGuardada,
            telefono: "",
            fechaRegistro: DateTime.UtcNow
        );

        // 4) Guardamos
        var creado = await _repo.CreateAsync(usuario); // si tu repo devuelve bool
        if (!creado) return (false, "No se pudo crear el usuario");

        // Autologin
        _auth.SignIn(email ?? userName);
        return (true, null);
    }

    // Login
    public async Task<(bool ok, string? error)> LoginAsync(string userName, string password)
    {
        var u = await _repo.GetByUserNameAsync(userName);
        if (u is null) return (false, "Usuario o contraseña inválidos");

        // 1) Extraemos hash y salt almacenados en formato "hashBase64:saltBase64"
        var partes = (u.Contraseña ?? "").Split(':');
        if (partes.Length != 2) return (false, "Formato de contraseña inválido");

        var expectedHash = Convert.FromBase64String(partes[0]);
        var salt = Convert.FromBase64String(partes[1]);

        // 2) Verificamos con los tres parámetros que requiere tu hasher
        var ok = _hasher.Verify(password, salt, expectedHash);
        if (!ok) return (false, "Usuario o contraseña inválidos");
        _auth.SignIn(u.Email ?? userName);
        return (true, null);
    }

    public void Logout() => _auth.SignOut();
}