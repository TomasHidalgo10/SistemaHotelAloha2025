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

            // Debe devolver: Id, Numero, TipoId, EstadoId, PrecioBase (+ opcional TipoNombre/EstadoNombre)
            var dt = _ado.GetAll();
            foreach (DataRow row in dt.Rows)
            {
                var h = new HabitacionModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Numero = row["Numero"] == DBNull.Value ? "" : row["Numero"]!.ToString()!,
                    TipoId = Convert.ToInt32(row["TipoId"]),
                    EstadoId = Convert.ToInt32(row["EstadoId"]),
                    PrecioNoche = Convert.ToDecimal(row["PrecioBase"]),

                    // Si tu modelo tiene estos campos de sólo lectura, descomentá:
                    // TipoNombre   = row.Table.Columns.Contains("TipoNombre") ? row["TipoNombre"]?.ToString() ?? "" : "",
                    // EstadoNombre = row.Table.Columns.Contains("EstadoNombre") ? row["EstadoNombre"]?.ToString() ?? "" : ""
                };
                _items.Add(h);
            }
            return _items;
        }

        public HabitacionModel Create(HabitacionModel x)
        {
            // Si en BD 'Numero' es INT, convertimos; si fuera VARCHAR, podés cambiar el ADO a aceptar string.
            var numeroInt = int.TryParse(x.Numero, out var n) ? n : 0;

            var newId = _ado.Create(
                numero: numeroInt,
                tipoId: x.TipoId,     // YA es int
                estadoId: x.EstadoId,   // YA es int
                precioBase: x.PrecioNoche
            );

            if (newId <= 0)
                throw new InvalidOperationException("No se pudo insertar la habitación en la BD.");

            // Refrescar y devolver la creada
            GetAll();
            return _items.First(i => i.Id == newId);
        }

        public void Update(HabitacionModel x)
        {
            // Si en BD 'Numero' es INT:
            var numeroInt = int.TryParse(x.Numero, out var n) ? n : 0;

            // Por si viene 0 y querés default "Disponible" por Id:
            // var estadoId = x.EstadoId == 0 ? LookupService.EstadoIdByNombre("Disponible") : x.EstadoId;
            var estadoId = x.EstadoId;

            var rows = _ado.Update(
                id: x.Id,
                numero: numeroInt,
                tipoId: x.TipoId,
                estadoId: estadoId,
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