using SistemaHotelAloha.AccesoDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.Desktop.Utils
{
    public static class LookupService
    {
        private static DataTable? _tipos;
        private static DataTable? _estados;

        // 🔹 Listas para los combos
        public static DataTable Tipos()
            => _tipos ??= new TipoHabitacionLookupAdo().GetAll();

        public static DataTable Estados()
            => _estados ??= new EstadoHabitacionLookupAdo().GetAll();

        // 🔹 Helper para bindear cualquier ComboBox
        public static void Bind(ComboBox combo, DataTable data)
        {
            combo.DataSource = null;
            combo.DisplayMember = "Nombre";
            combo.ValueMember = "Id";
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
     
            combo.DataSource = data;
        }
        private static DataTable? _estadosReserva;
        public static DataTable EstadosReserva()
            => _estadosReserva ??= new EstadoReservaLookupAdo().GetAll();
        public static int TipoIdByNombre(string nombre)
        => Tipos().AsEnumerable()
                  .Where(r => string.Equals(r.Field<string>("Nombre"), nombre, StringComparison.OrdinalIgnoreCase))
                  .Select(r => r.Field<int>("Id"))
                  .FirstOrDefault();

        public static int EstadoIdByNombre(string nombre)
            => Estados().AsEnumerable()
                        .Where(r => string.Equals(r.Field<string>("Nombre"), nombre, StringComparison.OrdinalIgnoreCase))
                        .Select(r => r.Field<int>("Id"))
                        .FirstOrDefault();

        public static string TipoNombreById(int id)
            => Tipos().AsEnumerable()
                      .Where(r => r.Field<int>("Id") == id)
                      .Select(r => r.Field<string>("Nombre"))
                      .FirstOrDefault() ?? "";

        public static string EstadoNombreById(int id)
            => Estados().AsEnumerable()
                        .Where(r => r.Field<int>("Id") == id)
                        .Select(r => r.Field<string>("Nombre"))
                        .FirstOrDefault() ?? "";
        public static string EstadoReservaNombreById(int id)
        {
            var dt = EstadosReserva();
            foreach (System.Data.DataRow r in dt.Rows)
                if (Convert.ToInt32(r["Id"]) == id)
                    return r["Nombre"]?.ToString() ?? "";
            return "";
        }
    }
}