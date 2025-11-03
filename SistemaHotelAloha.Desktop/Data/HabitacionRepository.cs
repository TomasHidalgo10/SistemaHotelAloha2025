using SistemaHotelAloha.AccesoDatos; // <- tu ADO
using SistemaHotelAloha.Desktop.Models;
using SistemaHotelAloha.Dominio;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using HabitacionModel = SistemaHotelAloha.Desktop.Models.Habitacion;
using TipoHabitacionModel = SistemaHotelAloha.Desktop.Models.TipoHabitacion; // o Dominio, según tu caso

namespace SistemaHotelAloha.Desktop.Data
{
    public class HabitacionRepository
    {
        private readonly HabitacionAdoRepository _ado = new();

        private readonly List<HabitacionModel> _items = new();

        public IEnumerable<HabitacionModel> GetAll()
        {
            _items.Clear();

            var dt = _ado.GetAll(); // DataTable con: Id, Numero, TipoHabitacion, Estado, PrecioBase
            foreach (DataRow row in dt.Rows)
            {
                var h = new HabitacionModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Numero = row["Numero"] == DBNull.Value ? "" : row["Numero"]!.ToString()!,
                    // Si tu enum es int en la DB:
                    Tipo = row["TipoHabitacion"]?.ToString() ?? "Standard",
                    Estado = row["Estado"]?.ToString() ?? "Disponible",
                    PrecioNoche = Convert.ToDecimal(row["PrecioBase"])
                };
                _items.Add(h);
            }
            return _items;
        }

        public HabitacionModel Create(HabitacionModel x)
        {
            // Convertir Numero (string) a int de forma segura
            var numeroInt = int.TryParse(x.Numero, out var n) ? n : 0;

            // Convertir Tipo (string) a int de forma segura (si se guarda como número en la BD)
            var tipoInt = int.TryParse(x.Tipo, out var t) ? t : 0;

            var newId = _ado.Create(
                numero: numeroInt,
                tipoHabitacion: tipoInt,
                estado: string.IsNullOrWhiteSpace(x.Estado) ? "Disponible" : x.Estado,
                precioBase: x.PrecioNoche
            );

            if (newId <= 0)
                throw new InvalidOperationException("No se pudo insertar la habitación en la BD.");

            // Refresca la lista desde la BD
            GetAll();
            return _items.First(i => i.Id == newId);
        }

        public void Update(HabitacionModel x)
        {
            // Conversión segura de string → int
            var numeroInt = int.TryParse(x.Numero, out var n) ? n : 0;
            var tipoInt = int.TryParse(x.Tipo, out var t) ? t : 0;

            var rows = _ado.Update(
                id: x.Id,
                numero: numeroInt,
                tipoHabitacion: tipoInt,
                estado: string.IsNullOrWhiteSpace(x.Estado) ? "Disponible" : x.Estado,
                precioBase: x.PrecioNoche
            );

            if (rows <= 0)
                throw new InvalidOperationException("No se pudo actualizar la habitación en la BD.");

            GetAll();
        }

        public void Delete(int id)
        {
            var rows = _ado.Delete(id);
            if (rows <= 0)
                throw new InvalidOperationException("No se pudo eliminar la habitación en la BD.");

            GetAll();
        }
    }
}