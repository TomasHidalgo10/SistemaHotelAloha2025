namespace SistemaHotelAloha.Desktop.Models
{
    public class Habitacion
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;     
        public string Estado { get; set; } = "Disponible";   // Disponible, Ocupada, Limpieza, Mantenimiento
        public decimal PrecioNoche { get; set; }
    }
}
