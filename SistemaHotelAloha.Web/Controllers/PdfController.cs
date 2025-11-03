// SistemaHotelAloha.Web/Controllers/PdfController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaHotelAloha.AccesoDatos;
using System.Text;

namespace SistemaHotelAloha.Web.Controllers
{
    [Route("pdf")]
    public class PdfController : Controller
    {
        private readonly ReservasAdoRepository _repo;
        private readonly ILogger<PdfController> _log;

        public PdfController(ReservasAdoRepository repo, ILogger<PdfController> log)
        {
            _repo = repo;
            _log = log;
        }

        // GET /pdf/reserva/123
        [HttpGet("reserva/{id:int}")]
        public IActionResult Reserva(int id)
        {
            try
            {
                // En una app real: sacar el userId del contexto. Para demo, 1.
                var userId = 1;

                var (cab, lineas) = _repo.ObtenerReservaParaPdfConServicios(id, userId);

                // Generación mínima: exportamos texto estructurado (descarga .pdf).
                // Si usás iText7/QuestPDF, decime y lo reemplazo por PDF real con formato.
                var sb = new StringBuilder();
                sb.AppendLine("HOTEL ALOHA");
                sb.AppendLine($"Reserva: #{cab.Id}");
                sb.AppendLine($"Huésped: {cab.Huesped ?? "-"}");
                sb.AppendLine($"Habitación: {cab.Habitacion}  Cap: {cab.Capacidad}");
                sb.AppendLine($"Desde: {cab.Desde:dd/MM/yyyy}  Hasta: {cab.Hasta:dd/MM/yyyy}");
                sb.AppendLine($"Precio/Noche: {cab.PrecioNoche:C}");
                sb.AppendLine();

                if (lineas.Count > 0)
                {
                    sb.AppendLine("Servicios:");
                    foreach (var l in lineas)
                        sb.AppendLine($" - {l.Nombre} x{l.Cantidad} ({l.PrecioTipo}): {l.Subtotal:C}");
                    sb.AppendLine();
                }

                sb.AppendLine($"TOTAL: {cab.Total:C}");
                sb.AppendLine();
                sb.AppendLine("¡Gracias por su reserva!");

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                // Nota: esto no es un PDF “real”, pero compila sin paquetes externos.
                return File(bytes, "application/octet-stream", $"reserva_{cab.Id}.pdf");
            }
            catch (System.Exception ex)
            {
                _log.LogError(ex, "Error generando PDF de reserva {Id}", id);
                return BadRequest("No se pudo generar el PDF.");
            }
        }
    }
}
