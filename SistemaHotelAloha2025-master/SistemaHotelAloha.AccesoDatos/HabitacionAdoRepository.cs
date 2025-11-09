using System;
using System.Data;
using MySql.Data.MySqlClient;
using SistemaHotelAloha.AccesoDatos.Infra;

namespace SistemaHotelAloha.AccesoDatos
{
    /// <summary>
    /// Repositorio ADO.NET para 'habitaciones' compatible con dos esquemas:
    /// 1) Esquema NORMALIZADO (Nico): habitaciones(TipoId, EstadoId) + tablas tipo_habitacion y estado_habitacion.
    /// 2) Esquema PLANO (Tsho): habitaciones(TipoHabitacion, Estado).
    ///
    /// Convenciones:
    /// - Create  => devuelve nuevo Id (>0). Si error => 0
    /// - Update  => filas afectadas (>=1). Si error => 0
    /// - Delete  => filas afectadas (>=1). Si error => 0
    /// - GetAll  => DataTable (columnas dependen del esquema)
    /// </summary>
    public class HabitacionAdoRepository
    {
        private enum SchemaMode { Normalized, Flat }

        /* =========================================================
         * PUBLIC - GET ALL
         * ========================================================= */
        public DataTable GetAll()
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();
            var mode = DetectSchemaMode(cn);

            var dt = new DataTable();

            if (mode == SchemaMode.Normalized)
            {
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
                da.Fill(dt);
            }
            else
            {
                // Esquema plano
                using var da = new MySqlDataAdapter(@"
                    SELECT
                        Id,
                        Numero,
                        TipoHabitacion,
                        Estado,
                        PrecioBase
                    FROM habitaciones
                    ORDER BY Id DESC;", cn);
                da.Fill(dt);
            }

            return dt;
        }

        /* =========================================================
         * PUBLIC - CREATE (variantes)
         * ========================================================= */

        // Esquema normalizado: IDs
        public int Create(int numero, int tipoId, int estadoId, decimal precioBase)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();
            var mode = DetectSchemaMode(cn);

            try
            {
                if (mode == SchemaMode.Normalized)
                {
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
                else
                {
                    // En plano, 'tipoId' se asume como código entero de TipoHabitacion y 'estadoId' no aplica.
                    using var cmd = new MySqlCommand(@"
                        INSERT INTO habitaciones (Numero, TipoHabitacion, Estado, PrecioBase)
                        VALUES (@Numero, @Tipo, @Estado, @Precio);
                        SELECT LAST_INSERT_ID();", cn);
                    cmd.Parameters.AddWithValue("@Numero", numero);
                    cmd.Parameters.AddWithValue("@Tipo", tipoId); // reutilizamos tipoId como código
                    cmd.Parameters.AddWithValue("@Estado", "Disponible"); // estado por defecto si no se especifica
                    cmd.Parameters.AddWithValue("@Precio", precioBase);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch
            {
                return 0;
            }
        }

        // Esquema plano: código + estado string (funcionalidad Tsho)
        public int Create(int numero, int tipoHabitacion, string estado, decimal precioBase)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();
            var mode = DetectSchemaMode(cn);

            try
            {
                if (mode == SchemaMode.Flat)
                {
                    using var cmd = new MySqlCommand(@"
                        INSERT INTO habitaciones (Numero, TipoHabitacion, Estado, PrecioBase)
                        VALUES (@Numero, @Tipo, @Estado, @Precio);
                        SELECT LAST_INSERT_ID();", cn);
                    cmd.Parameters.AddWithValue("@Numero", numero);
                    cmd.Parameters.AddWithValue("@Tipo", tipoHabitacion);
                    cmd.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Disponible" : estado);
                    cmd.Parameters.AddWithValue("@Precio", precioBase);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    // En normalizado, intentamos mapear: estado string -> EstadoId; tipoHabitacion (código) lo usamos como TipoId.
                    int estadoId = ResolveEstadoId(string.IsNullOrWhiteSpace(estado) ? "Disponible" : estado, cn);
                    using var cmd = new MySqlCommand(@"
                        INSERT INTO habitaciones (Numero, TipoId, EstadoId, PrecioBase)
                        VALUES (@Numero, @TipoId, @EstadoId, @Precio);
                        SELECT LAST_INSERT_ID();", cn);
                    cmd.Parameters.AddWithValue("@Numero", numero);
                    cmd.Parameters.AddWithValue("@TipoId", tipoHabitacion); // asumimos que el código coincide con un Id válido
                    cmd.Parameters.AddWithValue("@EstadoId", estadoId);
                    cmd.Parameters.AddWithValue("@Precio", precioBase);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch
            {
                return 0;
            }
        }

        // Por nombres (funcionalidad Nico)
        public int CreateByNames(int numero, string tipoNombre, string estadoNombre, decimal precioBase)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();
            var mode = DetectSchemaMode(cn);

            try
            {
                if (mode == SchemaMode.Normalized)
                {
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
                else
                {
                    // En plano no hay FK. Intentamos mapear tipoNombre a un código si existe la tabla auxiliar; si no, lo dejamos en 0.
                    int tipoCodigo = TryGetTipoIdIfAuxTableExists(tipoNombre, cn) ?? 0;
                    string estado = string.IsNullOrWhiteSpace(estadoNombre) ? "Disponible" : estadoNombre.Trim();

                    using var cmd = new MySqlCommand(@"
                        INSERT INTO habitaciones (Numero, TipoHabitacion, Estado, PrecioBase)
                        VALUES (@Numero, @Tipo, @Estado, @Precio);
                        SELECT LAST_INSERT_ID();", cn);
                    cmd.Parameters.AddWithValue("@Numero", numero);
                    cmd.Parameters.AddWithValue("@Tipo", tipoCodigo);
                    cmd.Parameters.AddWithValue("@Estado", estado);
                    cmd.Parameters.AddWithValue("@Precio", precioBase);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch
            {
                return 0;
            }
        }

        /* =========================================================
         * PUBLIC - UPDATE (variantes)
         * ========================================================= */

        // Normalizado: IDs
        public int Update(int id, int numero, int tipoId, int estadoId, decimal precioBase)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();
            var mode = DetectSchemaMode(cn);

            try
            {
                if (mode == SchemaMode.Normalized)
                {
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
                else
                {
                    // En plano: usamos tipoId como código y el estado se preserva tal cual si no se cambia aquí
                    using var cmd = new MySqlCommand(@"
                        UPDATE habitaciones
                           SET Numero=@Numero,
                               TipoHabitacion=@Tipo,
                               PrecioBase=@Precio
                         WHERE Id=@Id;", cn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Numero", numero);
                    cmd.Parameters.AddWithValue("@Tipo", tipoId);
                    cmd.Parameters.AddWithValue("@Precio", precioBase);
                    return cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                return 0;
            }
        }

        // Plano: código + estado string
        public int UpdateFlat(int id, int numero, int tipoHabitacion, string estado, decimal precioBase)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();
            var mode = DetectSchemaMode(cn);

            try
            {
                if (mode == SchemaMode.Flat)
                {
                    using var cmd = new MySqlCommand(@"
                        UPDATE habitaciones
                           SET Numero=@Numero,
                               TipoHabitacion=@Tipo,
                               Estado=@Estado,
                               PrecioBase=@Precio
                         WHERE Id=@Id;", cn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Numero", numero);
                    cmd.Parameters.AddWithValue("@Tipo", tipoHabitacion);
                    cmd.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Disponible" : estado);
                    cmd.Parameters.AddWithValue("@Precio", precioBase);
                    return cmd.ExecuteNonQuery();
                }
                else
                {
                    int estadoId = ResolveEstadoId(string.IsNullOrWhiteSpace(estado) ? "Disponible" : estado, cn);
                    using var cmd = new MySqlCommand(@"
                        UPDATE habitaciones
                           SET Numero=@Numero,
                               TipoId=@TipoId,
                               EstadoId=@EstadoId,
                               PrecioBase=@Precio
                         WHERE Id=@Id;", cn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Numero", numero);
                    cmd.Parameters.AddWithValue("@TipoId", tipoHabitacion); // usamos el código como Id
                    cmd.Parameters.AddWithValue("@EstadoId", estadoId);
                    cmd.Parameters.AddWithValue("@Precio", precioBase);
                    return cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                return 0;
            }
        }

        // Por nombres
        public int UpdateByNames(int id, int numero, string tipoNombre, string estadoNombre, decimal precioBase)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();
            var mode = DetectSchemaMode(cn);

            try
            {
                if (mode == SchemaMode.Normalized)
                {
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
                else
                {
                    int tipoCodigo = TryGetTipoIdIfAuxTableExists(tipoNombre, cn) ?? 0;
                    string estado = string.IsNullOrWhiteSpace(estadoNombre) ? "Disponible" : estadoNombre.Trim();

                    using var cmd = new MySqlCommand(@"
                        UPDATE habitaciones
                           SET Numero=@Numero,
                               TipoHabitacion=@Tipo,
                               Estado=@Estado,
                               PrecioBase=@Precio
                         WHERE Id=@Id;", cn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Numero", numero);
                    cmd.Parameters.AddWithValue("@Tipo", tipoCodigo);
                    cmd.Parameters.AddWithValue("@Estado", estado);
                    cmd.Parameters.AddWithValue("@Precio", precioBase);

                    return cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                return 0;
            }
        }

        /* =========================================================
         * PUBLIC - DELETE
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
         * HELPERS DE ESQUEMA Y RESOLUCIÓN
         * ========================================================= */

        private static SchemaMode DetectSchemaMode(MySqlConnection cn)
        {
            bool hasTipoId = HasColumn(cn, "habitaciones", "TipoId");
            bool hasEstadoId = HasColumn(cn, "habitaciones", "EstadoId");
            bool hasTipoHabitacion = HasColumn(cn, "habitaciones", "TipoHabitacion");
            bool hasEstado = HasColumn(cn, "habitaciones", "Estado");

            if (hasTipoId && hasEstadoId)
                return SchemaMode.Normalized;

            if (hasTipoHabitacion && hasEstado)
                return SchemaMode.Flat;

            // Por defecto, intentamos normalizado (más seguro para JOINs futuros)
            return SchemaMode.Normalized;
        }

        private static bool HasColumn(MySqlConnection cn, string table, string column)
        {
            using var cmd = new MySqlCommand(@"
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = @t
                  AND COLUMN_NAME = @c;", cn);
            cmd.Parameters.AddWithValue("@t", table);
            cmd.Parameters.AddWithValue("@c", column);
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

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

        /// <summary>
        /// Si existen tablas auxiliares, intenta obtener Id por nombre; si no existen, devuelve null.
        /// Útil para el esquema PLANO cuando queremos aceptar nombres.
        /// </summary>
        private static int? TryGetTipoIdIfAuxTableExists(string? tipoNombre, MySqlConnection cn)
        {
            if (!TableExists(cn, "tipo_habitacion")) return null;
            string nombre = string.IsNullOrWhiteSpace(tipoNombre) ? "Sin definir" : tipoNombre.Trim();

            using var get = new MySqlCommand("SELECT Id FROM tipo_habitacion WHERE Nombre=@n LIMIT 1;", cn);
            get.Parameters.AddWithValue("@n", nombre);
            var obj = get.ExecuteScalar();
            if (obj != null && obj != DBNull.Value) return Convert.ToInt32(obj);

            // crear si no existe
            using var ins = new MySqlCommand("INSERT INTO tipo_habitacion(Nombre) VALUES(@n); SELECT LAST_INSERT_ID();", cn);
            ins.Parameters.AddWithValue("@n", nombre);
            return Convert.ToInt32(ins.ExecuteScalar());
        }

        private static bool TableExists(MySqlConnection cn, string table)
        {
            using var cmd = new MySqlCommand(@"
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = @t;", cn);
            cmd.Parameters.AddWithValue("@t", table);
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }
}
