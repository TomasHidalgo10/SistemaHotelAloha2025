using System.ComponentModel;
using System.Linq;
using SistemaHotelAloha.Desktop.Models;

namespace SistemaHotelAloha.Desktop.Data
{
    public class HabitacionRepository
    {
        private readonly BindingList<Habitacion> _items = new();
        private int _nextId = 1;

        public BindingList<Habitacion> GetAll() => _items;

        public Habitacion Create(Habitacion h)
        {
            h.Id = _nextId++;
            _items.Add(h);
            return h;
        }

        public void Update(Habitacion updated)
        {
            var existing = _items.FirstOrDefault(x => x.Id == updated.Id);
            if (existing is null) return;
            existing.Numero = updated.Numero;
            existing.Tipo = updated.Tipo;
            existing.Estado = updated.Estado;
            existing.PrecioNoche = updated.PrecioNoche;
        }

        public void Delete(int id)
        {
            var existing = _items.FirstOrDefault(x => x.Id == id);
            if (existing is null) return;
            _items.Remove(existing);
        }
    }
}
