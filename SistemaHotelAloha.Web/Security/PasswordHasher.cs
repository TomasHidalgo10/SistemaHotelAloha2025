using System.Security.Cryptography;

namespace SistemaHotelAloha.Web.Security;

public class PasswordHasher
{
    public (byte[] hash, byte[] salt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Hash(password, salt);
        return (hash, salt);
    }

    public bool Verify(string password, byte[] salt, byte[] expectedHash)
    {
        var h = Hash(password, salt);
        return CryptographicOperations.FixedTimeEquals(h, expectedHash);
    }

    private static byte[] Hash(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(32);
    }
}
