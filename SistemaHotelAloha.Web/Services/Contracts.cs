using SistemaHotelAloha.Web.Models;

namespace SistemaHotelAloha.Web.Services;

public record BusquedaRequest(string Destino, DateOnly? CheckIn, DateOnly? CheckOut, int Adultos, int Ninos, int Pagina = 1, int TamPagina = 10, string? Orden = null, string[]? Amenidades = null, decimal? PrecioMin = null, decimal? PrecioMax = null);
public record BusquedaResult(List<Hotel> Hoteles, int Total);

public interface IHotelRepository
{
    Task<BusquedaResult> BuscarAsync(BusquedaRequest req, CancellationToken ct = default);
    Task<Hotel?> GetAsync(int id, CancellationToken ct = default);
}

public interface IBookingRepository
{
    Task<int> CrearReservaAsync(Reserva r, CancellationToken ct = default);
    Task<List<Reserva>> ReservasDeAsync(string userName, CancellationToken ct = default);
}
