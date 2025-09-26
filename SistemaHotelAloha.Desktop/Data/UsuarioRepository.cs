using System.Linq;
using System.ComponentModel;
using SistemaHotelAloha.Desktop.Models;

namespace SistemaHotelAloha.Desktop.Data
{
    public class UsuarioRepository
    {
        private readonly BindingList<Usuario> _usuarios = new BindingList<Usuario>();
        private int _nextId = 1;

        public BindingList<Usuario> GetAll() => _usuarios;

        public Usuario Create(Usuario u)
        {
            u.Id = _nextId++;
            _usuarios.Add(u);
            return u;
        }

        public void Update(Usuario updated)
        {
            var existing = _usuarios.FirstOrDefault(x => x.Id == updated.Id);
            if (existing is null) return;
            existing.Nombre = updated.Nombre;
            existing.Email = updated.Email;
            existing.Rol = updated.Rol;
        }

        public void Delete(int id)
        {
            var existing = _usuarios.FirstOrDefault(x => x.Id == id);
            if (existing is null) return;
            _usuarios.Remove(existing);
        }
    }
}
