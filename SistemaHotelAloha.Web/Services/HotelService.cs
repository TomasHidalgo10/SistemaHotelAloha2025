using SistemaHotelAloha.Web.Models;

namespace SistemaHotelAloha.Web.Services;

public class HotelService
{
    private readonly IHotelRepository _repo;
    public HotelService(IHotelRepository repo) => _repo = repo;

    public Task<BusquedaResult> BuscarAsync(BusquedaRequest req) => _repo.BuscarAsync(req);
    public Task<Hotel?> GetAsync(int id) => _repo.GetAsync(id);
}

public class BookingService
{
    private readonly IBookingRepository _repo;
    public BookingService(IBookingRepository repo) => _repo = repo;

    public Task<int> CrearAsync(Reserva r) => _repo.CrearReservaAsync(r);
    public Task<List<Reserva>> MiasAsync(string user) => _repo.ReservasDeAsync(user);
}
