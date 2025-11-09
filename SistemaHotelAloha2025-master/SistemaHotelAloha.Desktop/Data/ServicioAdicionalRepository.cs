using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using SistemaHotelAloha.Desktop.Models;
using SistemaHotelAloha.AccesoDatos;

namespace SistemaHotelAloha.Desktop.Data
{
    public class ServicioAdicionalRepository
    {
        private readonly BindingList<ServicioAdicional> _items = new();
        private readonly ServicioAdicionalAdoRepository _ado = new();

        public ServicioAdicionalRepository()
        {
            RefrescarDesdeBd();
        }

        private void RefrescarDesdeBd()
        {
            _items.Clear();
            var dt = _ado.GetAll();
            foreach (DataRow row in dt.Rows)
            {
                var s = new ServicioAdicional
                {
                    Id = row.Table.Columns.Contains("Id") ? Convert.ToInt32(row["Id"]) : 0,
                    Nombre = row.Table.Columns.Contains("Nombre") ? row["Nombre"]?.ToString() ?? "" : "",
                    Precio = row.Table.Columns.Contains("Precio")
                                ? Convert.ToDecimal(row["Precio"])
                                : (row.Table.Columns.Contains("Importe") ? Convert.ToDecimal(row["Importe"]) : 0m)
                };
                _items.Add(s);
            }
        }

        public BindingList<ServicioAdicional> GetAll() => _items;

        public ServicioAdicional Create(ServicioAdicional x)
        {
            var rows = _ado.Create(nombre: x.Nombre, precio: Convert.ToSingle(x.Precio), descripcion: "");
            if (rows <= 0) throw new InvalidOperationException("No se pudo insertar el servicio adicional.");

            RefrescarDesdeBd();
            return _items.Last();
        }

        public void Update(ServicioAdicional x)
        {
            var rows = _ado.Update(id: x.Id, nombre: x.Nombre, precio: Convert.ToSingle(x.Precio), descripcion: "");
            if (!rows) throw new InvalidOperationException("No se pudo actualizar el servicio adicional.");

            RefrescarDesdeBd();
        }

        public void Delete(int id)
        {
            var rows = _ado.Delete(id);
            if (!rows) throw new InvalidOperationException("No se pudo eliminar el servicio adicional.");

            RefrescarDesdeBd();
        }
    }
}