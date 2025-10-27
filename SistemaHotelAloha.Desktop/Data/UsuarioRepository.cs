using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using SistemaHotelAloha.Desktop.Models;
using SistemaHotelAloha.AccesoDatos;

namespace SistemaHotelAloha.Desktop.Data
{
    public class UsuarioRepository
    {
        private readonly BindingList<Usuario> _usuarios = new();
        private readonly UsuarioAdoRepository _ado = new();

        public UsuarioRepository()
        {
            RefrescarDesdeBd();
        }

        private void RefrescarDesdeBd()
        {
            _usuarios.Clear();
            var dt = _ado.GetAll();
            foreach (DataRow row in dt.Rows)
            {
                var u = new Usuario
                {
                    Id = row.Table.Columns.Contains("Id") ? Convert.ToInt32(row["Id"]) : 0,
                    Nombre = row.Table.Columns.Contains("Nombre") ? row["Nombre"]?.ToString() ?? "" : "",
                    Email = row.Table.Columns.Contains("Email") ? row["Email"]?.ToString() ?? "" : "",
                    Rol = row.Table.Columns.Contains("Rol") ? row["Rol"]?.ToString() ?? "" : "" // si no existe, queda vacío
                };
                _usuarios.Add(u);
            }
        }

        public BindingList<Usuario> GetAll() => _usuarios;

        public Usuario Create(Usuario u)
        {
            if (string.IsNullOrWhiteSpace(u.Nombre))
                throw new InvalidOperationException("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(u.Email))
                throw new InvalidOperationException("El email es obligatorio.");

            var result = _ado.Create(
                nombre: u.Nombre,
                apellido: "",
                email: u.Email,
                contraseña: "",     // el ADO pone "changeme" si columna NOT NULL
                telefono: "",
                fechaRegistro: DateTime.UtcNow,
                activo: true
            );

            if (result == -1062)
                throw new InvalidOperationException("El email ya está registrado. Usá otro email.");
            EnsureSuccess(result, "No se pudo insertar el usuario en la BD.");

            RefrescarDesdeBd();
            return _usuarios.Last();
        }

        public void Update(Usuario updated)
        {
            var result = _ado.Update(
                id: updated.Id,
                nombre: updated.Nombre,
                apellido: "",
                email: updated.Email,
                contraseña: "",
                telefono: "",
                fechaRegistro: DateTime.UtcNow,
                activo: true
            );

            if (result == -1062)
                throw new InvalidOperationException("Otro usuario ya tiene ese email.");
            EnsureSuccess(result, "No se pudo actualizar el usuario en la BD.");

            RefrescarDesdeBd();
        }

        public void Delete(int id)
        {
            var result = _ado.Delete(id);
            EnsureSuccess(result, "No se pudo eliminar el usuario en la BD.");
            RefrescarDesdeBd();
        }

        // ---------- Helpers ----------
        private static void EnsureSuccess(int rows, string message)
        {
            if (rows <= 0) throw new InvalidOperationException(message);
        }
    }
}