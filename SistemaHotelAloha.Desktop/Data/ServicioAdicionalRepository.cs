using System.ComponentModel;
using System.Linq;
using SistemaHotelAloha.Desktop.Models;

namespace SistemaHotelAloha.Desktop.Data
{
    public class ServicioAdicionalRepository
    {
        private readonly BindingList<ServicioAdicional> _items = new();
        private int _nextId = 1;

        public BindingList<ServicioAdicional> GetAll() => _items;

        public ServicioAdicional Create(ServicioAdicional s)
        {
            s.Id = _nextId++;
            _items.Add(s);
            return s;
        }

        public void Update(ServicioAdicional updated)
        {
            var existing = _items.FirstOrDefault(x => x.Id == updated.Id);
            if (existing is null) return;
            existing.Nombre = updated.Nombre;
            existing.Precio = updated.Precio;
        }

        public void Delete(int id)
        {
            var existing = _items.FirstOrDefault(x => x.Id == id);
            if (existing is null) return;
            _items.Remove(existing);
        }
    }
}
