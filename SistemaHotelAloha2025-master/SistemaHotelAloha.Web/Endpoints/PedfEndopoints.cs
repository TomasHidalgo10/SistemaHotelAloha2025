using SistemaHotelAloha.AccesoDatos;
using System.Text;

namespace SistemaHotelAloha.Web.Endpoints
{
    public static class PdfEndpoints
    {
        public static void MapPdfEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/pdf/reserva/{id:int}", (int id, ReservasAdoRepository repo) =>
            {
                var userId = 1;
                var (cab, lineas) = repo.ObtenerReservaParaPdfConServicios(id, userId);

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
                return Results.File(bytes, "application/octet-stream", $"reserva_{cab.Id}.pdf");
            });
        }
    }
}