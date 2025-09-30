namespace SistemaHotelAloha.Web.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public byte[] PasswordHash { get; set; } = System.Array.Empty<byte>();
    public byte[] Salt { get; set; } = System.Array.Empty<byte>();
    public System.DateTime CreatedAt { get; set; }
}
