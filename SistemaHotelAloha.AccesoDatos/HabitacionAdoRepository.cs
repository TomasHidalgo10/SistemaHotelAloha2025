using System;
using System.Data;
using MySql.Data.MySqlClient;
using SistemaHotelAloha.AccesoDatos.Infra;

namespace SistemaHotelAloha.AccesoDatos
{
    /// <summary>
    /// ADO.NET para tabla 'habitaciones'
    /// Convenciones:
    /// - Create  => devuelve nuevo Id (>0). Si error => 0
    /// - Update  => filas afectadas (>=1). Si error => 0
    /// - Delete  => filas afectadas (>=1). Si error => 0
    /// - GetAll  => DataTable
    /// </summary>
    public class HabitacionAdoRepository
    {
        /* =========================================================
         * GET ALL
         * Trae Ids + nombres de catálogos (para mostrar en grillas)
         * ========================================================= */
        public DataTable GetAll()
        {
            using var cn = MySqlConnectionFactory.Create();
            using var da = new MySqlDataAdapter(@"
                SELECT
                    h.Id,
                    h.Numero,
                    h.TipoId,
                    t.Nombre  AS TipoNombre,
                    h.EstadoId,
                    e.Nombre  AS EstadoNombre,
                    h.PrecioBase
                FROM habitaciones h
                JOIN tipo_habitacion   t ON t.Id = h.TipoId
                JOIN estado_habitacion e ON e.Id = h.EstadoId
                ORDER BY h.Id DESC;", cn);

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        /* =========================================================
         * CREATE (con IDs)
         * ========================================================= */
        public int Create(int numero, int tipoId, int estadoId, decimal precioBase)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
                INSERT INTO habitaciones (Numero, TipoId, EstadoId, PrecioBase)
                VALUES (@Numero, @TipoId, @EstadoId, @Precio);
                SELECT LAST_INSERT_ID();", cn);

            cmd.Parameters.AddWithValue("@Numero", numero);
            cmd.Parameters.AddWithValue("@TipoId", tipoId);
            cmd.Parameters.AddWithValue("@EstadoId", estadoId);
            cmd.Parameters.AddWithValue("@Precio", precioBase);

            cn.Open();
            try
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch
            {
                return 0;
            }
        }

        /* =========================================================
         * CREATE (por nombre)
         * Permite seguir usando strings "Doble", "Ocupada", etc.
         * ========================================================= */
        public int Create(string numero, string tipoNombre, string estadoNombre, decimal precioBase)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();

            int tipoId = ResolveTipoId(tipoNombre, cn);
            int estadoId = ResolveEstadoId(estadoNombre, cn);

            using var cmd = new MySqlCommand(@"
                INSERT INTO habitaciones (Numero, TipoId, EstadoId, PrecioBase)
                VALUES (@Numero, @TipoId, @EstadoId, @Precio);
                SELECT LAST_INSERT_ID();", cn);

            cmd.Parameters.AddWithValue("@Numero", numero);
            cmd.Parameters.AddWithValue("@TipoId", tipoId);
            cmd.Parameters.AddWithValue("@EstadoId", estadoId);
            cmd.Parameters.AddWithValue("@Precio", precioBase);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        /* =========================================================
         * UPDATE (con IDs)
         * ========================================================= */
        public int Update(int id, int numero, int tipoId, int estadoId, decimal precioBase)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
                UPDATE habitaciones
                   SET Numero=@Numero,
                       TipoId=@TipoId,
                       EstadoId=@EstadoId,
                       PrecioBase=@Precio
                 WHERE Id=@Id;", cn);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Numero", numero);
            cmd.Parameters.AddWithValue("@TipoId", tipoId);
            cmd.Parameters.AddWithValue("@EstadoId", estadoId);
            cmd.Parameters.AddWithValue("@Precio", precioBase);

            cn.Open();
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch
            {
                return 0;
            }
        }

        /* =========================================================
         * UPDATE (por nombre)
         * ========================================================= */
        public int Update(int id, string numero, string tipoNombre, string estadoNombre, decimal precioBase)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();

            int tipoId = ResolveTipoId(tipoNombre, cn);
            int estadoId = ResolveEstadoId(estadoNombre, cn);

            using var cmd = new MySqlCommand(@"
                UPDATE habitaciones
                   SET Numero=@Numero,
                       TipoId=@TipoId,
                       EstadoId=@EstadoId,
                       PrecioBase=@Precio
                 WHERE Id=@Id;", cn);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Numero", numero);
            cmd.Parameters.AddWithValue("@TipoId", tipoId);
            cmd.Parameters.AddWithValue("@EstadoId", estadoId);
            cmd.Parameters.AddWithValue("@Precio", precioBase);

            return cmd.ExecuteNonQuery();
        }

        /* =========================================================
         * DELETE
         * ========================================================= */
        public int Delete(int id)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand("DELETE FROM habitaciones WHERE Id=@Id;", cn);
            cmd.Parameters.AddWithValue("@Id", id);

            cn.Open();
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch
            {
                return 0;
            }
        }

        /* =========================================================
         * HELPERS
         * ========================================================= */

        private static int ResolveTipoId(string? tipoNombre, MySqlConnection cn)
        {
            string nombre = string.IsNullOrWhiteSpace(tipoNombre) ? "Sin definir" : tipoNombre.Trim();

            using (var get = new MySqlCommand("SELECT Id FROM tipo_habitacion WHERE Nombre=@n LIMIT 1;", cn))
            {
                get.Parameters.AddWithValue("@n", nombre);
                var obj = get.ExecuteScalar();
                if (obj != null && obj != DBNull.Value)
                    return Convert.ToInt32(obj);
            }

            using (var ins = new MySqlCommand("INSERT INTO tipo_habitacion(Nombre) VALUES(@n); SELECT LAST_INSERT_ID();", cn))
            {
                ins.Parameters.AddWithValue("@n", nombre);
                return Convert.ToInt32(ins.ExecuteScalar());
            }
        }

        private static int ResolveEstadoId(string? estadoNombre, MySqlConnection cn)
        {
            string nombre = string.IsNullOrWhiteSpace(estadoNombre) ? "Disponible" : estadoNombre.Trim();

            using (var get = new MySqlCommand("SELECT Id FROM estado_habitacion WHERE Nombre=@n LIMIT 1;", cn))
            {
                get.Parameters.AddWithValue("@n", nombre);
                var obj = get.ExecuteScalar();
                if (obj != null && obj != DBNull.Value)
                    return Convert.ToInt32(obj);
            }

            using (var ins = new MySqlCommand("INSERT INTO estado_habitacion(Nombre) VALUES(@n); SELECT LAST_INSERT_ID();", cn))
            {
                ins.Parameters.AddWithValue("@n", nombre);
                return Convert.ToInt32(ins.ExecuteScalar());
            }
        }
    }
}