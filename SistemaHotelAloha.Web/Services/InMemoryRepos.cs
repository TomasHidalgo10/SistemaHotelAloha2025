using SistemaHotelAloha.Web.Models;

namespace SistemaHotelAloha.Web.Services;

public class InMemoryHotelRepository : IHotelRepository
{
    private readonly List<Hotel> _data;

    public InMemoryHotelRepository()
    {
        _data = Seed();
    }

    public Task<BusquedaResult> BuscarAsync(BusquedaRequest req, CancellationToken ct = default)
    {
        IEnumerable<Hotel> q = _data;
        if (!string.IsNullOrWhiteSpace(req.Destino))
            q = q.Where(h => (h.Direccion.Ciudad + " " + h.Direccion.Provincia + " " + h.Nombre).ToLower().Contains(req.Destino.ToLower()));
        if (req.Amenidades?.Length > 0)
            q = q.Where(h => req.Amenidades!.All(a => h.Amenidades.Any(x => x.Nombre.Equals(a, StringComparison.OrdinalIgnoreCase))));
        if (req.PrecioMin is not null)
            q = q.Where(h => h.Habitaciones.Any(ht => ht.PrecioBasePorNoche >= req.PrecioMin));
        if (req.PrecioMax is not null)
            q = q.Where(h => h.Habitaciones.Any(ht => ht.PrecioBasePorNoche <= req.PrecioMax));
        if (req.Orden == "precio")
            q = q.OrderBy(h => h.Habitaciones.Min(ht => ht.PrecioBasePorNoche));
        else if (req.Orden == "calificacion")
            q = q.OrderByDescending(h => h.Calificacion);

        var total = q.Count();
        q = q.Skip((req.Pagina - 1) * req.TamPagina).Take(req.TamPagina);
        return Task.FromResult(new BusquedaResult(q.ToList(), total));
    }

    public Task<Hotel?> GetAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_data.FirstOrDefault(h => h.Id == id));

    private static List<Hotel> Seed()
    {
        var list = new List<Hotel>();
        for (int i = 1; i <= 25; i++)
        {
            list.Add(new Hotel
            {
                Id = i,
                Nombre = $"Hotel ALOHA {i}",
                Direccion = new Direccion { Pais = "Argentina", Provincia = "Buenos Aires", Ciudad = "CABA", Calle = "Av. Demo", Numero = $"{100+i}" },
                Calificacion = Math.Round(3 + 2 * (i % 5) / 4.0, 1),
                DescripcionCorta = "Cerca del centro, desayuno incluido y Wi‑Fi gratis.",
                Amenidades = new() { new Amenidad{ Nombre="WiFi" }, new Amenidad{ Nombre="Desayuno" }, new Amenidad{ Nombre="Pileta" } },
                Fotos = new() { new Foto{ Url=$"/img/h{i%5+1}.png", Alt="Fachada"} },
                Habitaciones = new()
                {
                    new HabitacionTipo{ Id = i*10+1, Nombre="Standard", Capacidad=2, PrecioBasePorNoche= 45000 + 1000*i },
                    new HabitacionTipo{ Id = i*10+2, Nombre="Suite", Capacidad=3, PrecioBasePorNoche= 85000 + 1500*i }
                }
            });
        }
        return list;
    }
}

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly List<Reserva> _reservas = new();

    public Task<int> CrearReservaAsync(Reserva r, CancellationToken ct = default)
    {
        r.Id = _reservas.Count + 1;
        _reservas.Add(r);
        return Task.FromResult(r.Id);
    }

    public Task<List<Reserva>> ReservasDeAsync(string userName, CancellationToken ct = default)
        => Task.FromResult(_reservas.Where(r => r.Usuario.Equals(userName, StringComparison.OrdinalIgnoreCase)).ToList());
}
