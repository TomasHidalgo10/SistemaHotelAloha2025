namespace SistemaHotelAloha.Web.Models;

public class Direccion
{
    public string Pais { get; set; } = "";
    public string Provincia { get; set; } = "";
    public string Ciudad { get; set; } = "";
    public string Calle { get; set; } = "";
    public string Numero { get; set; } = "";
    public string CodigoPostal { get; set; } = "";
}

public class Foto
{
    public string Url { get; set; } = "";
    public string? Alt { get; set; }
}

public class Amenidad
{
    public string Nombre { get; set; } = "";
}

public class HabitacionTipo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int Capacidad { get; set; }
    public decimal PrecioBasePorNoche { get; set; }
}

public class Hotel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public Direccion Direccion { get; set; } = new();
    public double Calificacion { get; set; }
    public List<Amenidad> Amenidades { get; set; } = new();
    public List<Foto> Fotos { get; set; } = new();
    public List<HabitacionTipo> Habitaciones { get; set; } = new();
    public string DescripcionCorta { get; set; } = "";
}
