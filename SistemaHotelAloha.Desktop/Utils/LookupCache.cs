using SistemaHotelAloha.AccesoDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.Desktop.Utils
{
    public static class LookupCache
    {
        private static bool _loaded;
        private static readonly Dictionary<int, string> _tipoById = new();
        private static readonly Dictionary<string, int> _tipoByName = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<int, string> _estadoById = new();
        private static readonly Dictionary<string, int> _estadoByName = new(StringComparer.OrdinalIgnoreCase);

        public static void Initialize()
        {
            if (_loaded) return;

            // Tipos
            var dtTipos = new TipoHabitacionLookupAdo().GetAll(); // SELECT Id, Nombre FROM tipo_habitacion
            foreach (DataRow r in dtTipos.Rows)
            {
                var id = Convert.ToInt32(r["Id"]);
                var name = Convert.ToString(r["Nombre"]) ?? "";
                if (!_tipoById.ContainsKey(id)) _tipoById[id] = name;
                if (!_tipoByName.ContainsKey(name)) _tipoByName[name] = id;
            }

            // Estados habitación
            var dtEstados = new EstadoHabitacionLookupAdo().GetAll(); // SELECT Id, Nombre FROM estado_habitacion
            foreach (DataRow r in dtEstados.Rows)
            {
                var id = Convert.ToInt32(r["Id"]);
                var name = Convert.ToString(r["Nombre"]) ?? "";
                if (!_estadoById.ContainsKey(id)) _estadoById[id] = name;
                if (!_estadoByName.ContainsKey(name)) _estadoByName[name] = id;
            }

            _loaded = true;
        }

        private static void EnsureLoaded()
        {
            if (!_loaded) Initialize();
        }

        public static string? GetTipoNombre(int id)
        {
            EnsureLoaded();
            return _tipoById.TryGetValue(id, out var name) ? name : null;
        }

        public static int? GetTipoId(string? name)
        {
            EnsureLoaded();
            if (string.IsNullOrWhiteSpace(name)) return null;
            return _tipoByName.TryGetValue(name, out var id) ? id : (int?)null;
        }

        public static string? GetEstadoNombre(int id)
        {
            EnsureLoaded();
            return _estadoById.TryGetValue(id, out var name) ? name : null;
        }

        public static int? GetEstadoId(string? name)
        {
            EnsureLoaded();
            if (string.IsNullOrWhiteSpace(name)) return null;
            return _estadoByName.TryGetValue(name, out var id) ? id : (int?)null;
        }
    }
}