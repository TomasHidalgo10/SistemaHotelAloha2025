namespace SistemaHotelAloha.Web.Models;

public class Huesped
{
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Telefono { get; set; }
}

public class Reserva
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public int HabitacionTipoId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int Adultos { get; set; }
    public int Ninos { get; set; }
    public decimal Total { get; set; }
    public string Usuario { get; set; } = ""; 
    public Huesped Huesped { get; set; } = new();
}
