namespace SistemaHotelAloha.Web.Controllers
{
    public class ClienteUpdateDto
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Email { get; set; }
        public string? Contraseña { get; set; }
        public string? Telefono { get; set; }
        public string? Dni { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? Nacionalidad { get; set; }
    }
}
