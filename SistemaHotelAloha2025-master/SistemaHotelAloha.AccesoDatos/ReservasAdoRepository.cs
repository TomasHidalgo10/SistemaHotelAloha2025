
using MySql.Data.MySqlClient;
using SistemaHotelAloha.AccesoDatos.Infra;
using SistemaHotelAloha.AccesoDatos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SistemaHotelAloha.AccesoDatos.ReservasAdoRepository;

namespace SistemaHotelAloha.AccesoDatos
{
    public class ReservasAdoRepository
    {
        // ======== Compatibilidad con ambos estilos de conexión ========
        private readonly string? _connStr;
        public ReservasAdoRepository() { _connStr = null; } // Usará MySqlConnectionFactory.Create()
        public ReservasAdoRepository(string connectionString) { _connStr = connectionString; }

        private MySqlConnection CreateConnection()
            => string.IsNullOrWhiteSpace(_connStr)
               ? MySqlConnectionFactory.Create()
               : new MySqlConnection(_connStr!);

        // ======== DTO de Nico para grilla ========
        public sealed record ReservaDto(
            int Id,
            int HabitacionId,
            string HabitacionLabel,
            DateTime Desde,
            DateTime Hasta,
            decimal PrecioTotal,
            string Estado
        );

        // ======== Helpers de esquema dinámico ========
        private sealed class ReservaSchema
        {
            public string IdCol = "Id";
            public string UserIdCol = "UsuarioId";      // o IdUsuario
            public string HabIdCol = "HabitacionId";    // o IdHabitacion
            public string DesdeCol = "FechaDesde";      // variantes fecha_desde
            public string HastaCol = "FechaHasta";      // variantes fecha_hasta
            public string EstadoCol = "Estado";         // variantes estado
            public string TotalCol = "PrecioTotal";     // o Total / Precio
            public string? HuespedCol = "Huesped";      // puede no existir
        }

        private ReservaSchema DetectReservaSchema(MySqlConnection cn)
        {
            var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var cmd = new MySqlCommand(@"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'reservas';", cn))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read()) cols.Add(rd.GetString(0));
            }

            var sch = new ReservaSchema();
            sch.IdCol = cols.Contains("Id") ? "Id" : cols.Contains("id") ? "id" : "Id";

            // Usuario
            sch.UserIdCol = cols.Contains("UsuarioId") ? "UsuarioId" :
                            cols.Contains("IdUsuario") ? "IdUsuario" :
                            cols.Contains("user_id") ? "user_id" : "UsuarioId";
            // Habitación
            sch.HabIdCol = cols.Contains("HabitacionId") ? "HabitacionId" :
                           cols.Contains("IdHabitacion") ? "IdHabitacion" :
                           cols.Contains("habitacion_id") ? "habitacion_id" : "HabitacionId";
            // Fechas
            sch.DesdeCol = cols.Contains("FechaDesde") ? "FechaDesde" :
                           cols.Contains("fecha_desde") ? "fecha_desde" : "FechaDesde";
            sch.HastaCol = cols.Contains("FechaHasta") ? "FechaHasta" :
                           cols.Contains("fecha_hasta") ? "fecha_hasta" : "FechaHasta";
            // Estado
            sch.EstadoCol = cols.Contains("Estado") ? "Estado" :
                            cols.Contains("estado") ? "estado" : "Estado";
            // Total
            sch.TotalCol = cols.Contains("PrecioTotal") ? "PrecioTotal" :
                           cols.Contains("Total") ? "Total" :
                           cols.Contains("Precio") ? "Precio" : "PrecioTotal";
            // Huesped
            sch.HuespedCol = cols.Contains("Huesped") ? "Huesped" :
                             cols.Contains("huesped") ? "huesped" : null;

            return sch;
        }

        private static bool Overlaps(DateTime aDesde, DateTime aHasta, DateTime bDesde, DateTime bHasta)
            => (aDesde < bHasta) && (aHasta > bDesde);

        // ===================================================================
        // HABITACIONES DISPONIBLES (Nico)
        // ===================================================================
        public List<(int Id, string Label, decimal Precio)> GetHabitacionesDisponibles(DateTime desde, DateTime hasta)
        {
            using var cn = CreateConnection();
            using var cmd = new MySqlCommand(@"
            SELECT  h.Id,
                    CONCAT('Hab ', h.Numero, ' (', COALESCE(th.Nombre,'Standard'), ')') AS Label,
                    h.PrecioBase
            FROM Habitaciones h
            LEFT JOIN tipo_habitacion th ON th.Id = h.TipoId
            WHERE h.EstadoId IS NOT NULL
              AND NOT EXISTS
              (
                SELECT 1
                FROM Reservas r
                WHERE r.HabitacionId = h.Id
                  AND r.Estado <> 'Cancelada'
                  AND NOT (@Hasta <= r.FechaDesde OR @Desde >= r.FechaHasta)
              )
            ORDER BY h.Numero;", cn);

            cmd.Parameters.AddWithValue("@Desde", desde);
            cmd.Parameters.AddWithValue("@Hasta", hasta);

            cn.Open();
            using var rd = cmd.ExecuteReader();
            var list = new List<(int, string, decimal)>();
            while (rd.Read())
            {
                var id = rd.GetInt32("Id");
                var label = rd["Label"]?.ToString() ?? $"Hab {id}";
                var precio = Convert.ToDecimal(rd["PrecioBase"], CultureInfo.InvariantCulture);
                list.Add((id, label, precio));
            }
            return list;
        }

        // ===================================================================
        // DISPONIBILIDAD SIMPLE (Tsho) con detección de columnas
        // ===================================================================
        public bool HabitacionDisponible(int idHabitacion, DateTime fechaDesde, DateTime fechaHasta)
        {
            if (fechaHasta.Date <= fechaDesde.Date)
                throw new ArgumentException("El rango de fechas es inválido.");

            using var cn = CreateConnection();
            cn.Open();
            var s = DetectReservaSchema(cn);

            string sql = $@"
                SELECT NOT EXISTS (
                    SELECT 1
                    FROM reservas r
                    WHERE r.{s.HabIdCol} = @hab
                      AND r.{s.EstadoCol} <> 'Cancelada'
                      AND (@desde < r.{s.HastaCol}) AND (@hasta > r.{s.DesdeCol})
                );";

            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@hab", idHabitacion);
            cmd.Parameters.AddWithValue("@desde", fechaDesde.Date);
            cmd.Parameters.AddWithValue("@hasta", fechaHasta.Date);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        // ===================================================================
        // CREAR (Nico) - recibe precioTotal calculado y devuelve Id o -100
        // ===================================================================
        public int Crear(int usuarioId, int habitacionId, DateTime desde, DateTime hasta, decimal precioTotal)
        {
            using var cn = CreateConnection();
            cn.Open();
            var s = DetectReservaSchema(cn);

            // Chequeo solapamiento
            using (var chk = new MySqlCommand($@"
                SELECT COUNT(1)
                FROM Reservas
                WHERE {s.HabIdCol}=@H
                  AND {s.EstadoCol}<>'Cancelada'
                  AND NOT (@Hasta <= {s.DesdeCol} OR @Desde >= {s.HastaCol});", cn))
            {
                chk.Parameters.AddWithValue("@H", habitacionId);
                chk.Parameters.AddWithValue("@Desde", desde);
                chk.Parameters.AddWithValue("@Hasta", hasta);

                if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                    return -100;
            }

            using var cmd = new MySqlCommand($@"
                INSERT INTO Reservas ({s.UserIdCol}, {s.HabIdCol}, {s.DesdeCol}, {s.HastaCol}, {s.EstadoCol}, {s.TotalCol})
                VALUES (@U, @H, @Desde, @Hasta, 'Confirmada', @Precio);
                SELECT LAST_INSERT_ID();", cn);

            cmd.Parameters.AddWithValue("@U", usuarioId);
            cmd.Parameters.AddWithValue("@H", habitacionId);
            cmd.Parameters.AddWithValue("@Desde", desde);
            cmd.Parameters.AddWithValue("@Hasta", hasta);
            cmd.Parameters.AddWithValue("@Precio", precioTotal);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ===================================================================
        // CREAR (Tsho) - calcula noches*precioBase y devuelve Id
        // ===================================================================
        public int CrearReserva(int idUsuario, int idHabitacion, string huesped, DateTime fechaDesde, DateTime fechaHasta)
        {
            if (fechaHasta.Date <= fechaDesde.Date)
                throw new ArgumentException("El rango de fechas es inválido.");

            using var cn = CreateConnection();
            cn.Open();
            var s = DetectReservaSchema(cn);
            using var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);

            // Overlap
            using (var check = new MySqlCommand($@"
                SELECT COUNT(*)
                FROM reservas r
                WHERE r.{s.HabIdCol} = @hab
                  AND r.{s.EstadoCol} <> 'Cancelada'
                  AND (@desde < r.{s.HastaCol}) AND (@hasta > r.{s.DesdeCol});", cn, tx))
            {
                check.Parameters.AddWithValue("@hab", idHabitacion);
                check.Parameters.AddWithValue("@desde", fechaDesde.Date);
                check.Parameters.AddWithValue("@hasta", fechaHasta.Date);
                if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                    throw new InvalidOperationException("La habitación no está disponible en ese rango.");
            }

            // Precio base
            decimal precioBase;
            using (var p = new MySqlCommand(@"SELECT PrecioBase FROM habitaciones WHERE Id = @hab;", cn, tx))
            {
                p.Parameters.AddWithValue("@hab", idHabitacion);
                var obj = p.ExecuteScalar() ?? throw new InvalidOperationException("Habitación inexistente.");
                precioBase = Convert.ToDecimal(obj, CultureInfo.InvariantCulture);
            }

            var noches = (int)(fechaHasta.Date - fechaDesde.Date).TotalDays;
            if (noches <= 0) throw new InvalidOperationException("Checkout debe ser posterior al checkin.");
            var total = precioBase * noches;

            string huespedColumn = s.HuespedCol ?? "Huesped"; // si no existe, igual intentamos insertarlo como columna opcional
            var insertCols = new List<string> { s.UserIdCol, s.HabIdCol, s.DesdeCol, s.HastaCol, s.EstadoCol, s.TotalCol };
            var insertPars = new List<string> { "@uid", "@hab", "@desde", "@hasta", "'Confirmada'", "@total" };

            if (s.HuespedCol != null)
            {
                insertCols.Insert(2, s.HuespedCol);
                insertPars.Insert(2, "@huesped");
            }

            string sql = $@"
                INSERT INTO reservas ({string.Join(",", insertCols)})
                VALUES ({string.Join(",", insertPars)});
                SELECT LAST_INSERT_ID();";

            using (var ins = new MySqlCommand(sql, cn, tx))
            {
                ins.Parameters.AddWithValue("@uid", idUsuario);
                ins.Parameters.AddWithValue("@hab", idHabitacion);
                ins.Parameters.AddWithValue("@desde", fechaDesde.Date);
                ins.Parameters.AddWithValue("@hasta", fechaHasta.Date);
                ins.Parameters.AddWithValue("@total", total);
                if (s.HuespedCol != null) ins.Parameters.AddWithValue("@huesped", huesped ?? "");

                var newId = Convert.ToInt32(ins.ExecuteScalar());
                tx.Commit();
                return newId;
            }
        }

        // ===================================================================
        // LISTAR (Nico) por usuario - simple
        // ===================================================================
        public List<ReservaDto> GetByUsuario(int usuarioId)
        {
            using var cn = CreateConnection();
            using var cmd = new MySqlCommand(@"
            SELECT r.Id,
                   r.HabitacionId,
                   CONCAT('Hab ', h.Numero, ' (', COALESCE(th.Nombre,'Standard'), ')') AS HabLabel,
                   r.FechaDesde,
                   r.FechaHasta,
                   r.PrecioTotal,
                   r.Estado
            FROM Reservas r
            JOIN Habitaciones h   ON h.Id = r.HabitacionId
            LEFT JOIN tipo_habitacion th ON th.Id = h.TipoId
            WHERE r.UsuarioId=@U
            ORDER BY r.Id DESC;", cn);

            cmd.Parameters.AddWithValue("@U", usuarioId);
            cn.Open();

            using var rd = cmd.ExecuteReader();
            var result = new List<ReservaDto>();
            while (rd.Read())
            {
                result.Add(new ReservaDto(
                    Id: rd.GetInt32("Id"),
                    HabitacionId: rd.GetInt32("HabitacionId"),
                    HabitacionLabel: rd["HabLabel"]?.ToString() ?? "",
                    Desde: Convert.ToDateTime(rd["FechaDesde"]),
                    Hasta: Convert.ToDateTime(rd["FechaHasta"]),
                    PrecioTotal: Convert.ToDecimal(rd["PrecioTotal"], CultureInfo.InvariantCulture),
                    Estado: rd["Estado"]?.ToString() ?? "Confirmada"
                ));
            }
            return result;
        }

        // ===================================================================
        // LISTAR (Nico) con filtro opcional por estado
        // ===================================================================
        public List<ReservaDto> GetByUsuario(int usuarioId, string? estado = null)
        {
            using var cn = CreateConnection();
            var sql = @"
            SELECT r.Id,
                   r.HabitacionId,
                   CONCAT('Hab ', h.Numero, ' (', COALESCE(th.Nombre,'Standard'), ')') AS HabLabel,
                   r.FechaDesde,
                   r.FechaHasta,
                   r.PrecioTotal,
                   r.Estado
            FROM Reservas r
            JOIN Habitaciones h        ON h.Id = r.HabitacionId
            LEFT JOIN tipo_habitacion th ON th.Id = h.TipoId
            WHERE r.UsuarioId=@U";

            if (!string.IsNullOrWhiteSpace(estado))
                sql += " AND r.Estado=@E";

            sql += " ORDER BY r.Id DESC;";

            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@U", usuarioId);
            if (!string.IsNullOrWhiteSpace(estado))
                cmd.Parameters.AddWithValue("@E", estado);

            cn.Open();
            using var rd = cmd.ExecuteReader();
            var result = new List<ReservaDto>();
            while (rd.Read())
            {
                result.Add(new ReservaDto(
                    Id: rd.GetInt32("Id"),
                    HabitacionId: rd.GetInt32("HabitacionId"),
                    HabitacionLabel: rd["HabLabel"]?.ToString() ?? "",
                    Desde: Convert.ToDateTime(rd["FechaDesde"]),
                    Hasta: Convert.ToDateTime(rd["FechaHasta"]),
                    PrecioTotal: Convert.ToDecimal(rd["PrecioTotal"], CultureInfo.InvariantCulture),
                    Estado: rd["Estado"]?.ToString() ?? "Confirmada"
                ));
            }
            return result;
        }

        // ===================================================================
        // LISTAR (Tsho) - devuelve ReservaDTO (tu modelo existente)
        // ===================================================================
        public IEnumerable<ReservaDTO> ListarReservasDeUsuario(int idUsuario)
        {
            const string sql = @"
                SELECT r.Id, r.FechaDesde, r.FechaHasta, r.Estado, r.Total,
                       h.Numero, h.PrecioBase
                FROM reservas r
                JOIN habitaciones h ON h.Id = r.IdHabitacion
                WHERE r.IdUsuario = @uid
                ORDER BY r.FechaDesde DESC;";

            using var cn = CreateConnection();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@uid", idUsuario);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var desde = rd.GetDateTime("FechaDesde").Date;
                var hasta = rd.GetDateTime("FechaHasta").Date;
                var noches = (int)(hasta - desde).TotalDays;

                yield return new ReservaDTO
                {
                    Id = rd.GetInt32("Id"),
                    FechaDesde = desde,
                    FechaHasta = hasta,
                    Estado = rd.GetString("Estado"),
                    Total = Convert.ToDecimal(rd["Total"], CultureInfo.InvariantCulture),
                    Habitacion = $"Hab. {rd.GetInt32("Numero")}",
                    Destino = "Hotel Aloha",
                    Noches = noches < 0 ? 0 : noches,
                    PrecioNoche = Convert.ToDecimal(rd["PrecioBase"], CultureInfo.InvariantCulture)
                };
            }
        }

        // ===================================================================
        // CANCELAR (Nico / Tsho)
        // ===================================================================
        public bool Cancelar(int id, int usuarioId)
        {
            using var cn = CreateConnection();
            using var cmd = new MySqlCommand(@"
            UPDATE Reservas
            SET Estado='Cancelada'
            WHERE Id=@Id AND UsuarioId=@U;", cn);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@U", usuarioId);

            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool CancelarReserva(int idReserva, int idUsuario)
        {
            using var cn = CreateConnection();
            cn.Open();
            var s = DetectReservaSchema(cn);

            string sql = $@"
                UPDATE reservas
                   SET {s.EstadoCol} = 'Cancelada'
                 WHERE {s.IdCol} = @id
                   AND {s.UserIdCol} = @uid
                   AND {s.EstadoCol} <> 'Cancelada';";

            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", idReserva);
            cmd.Parameters.AddWithValue("@uid", idUsuario);
            return cmd.ExecuteNonQuery() > 0;
        }

        // ===================================================================
        // ACTUALIZAR (Nico) con control de solapamiento
        // ===================================================================
        public int Actualizar(int id, int usuarioId, int habitacionId, DateTime desde, DateTime hasta, decimal precioTotal)
        {
            using var cn = CreateConnection();
            cn.Open();
            var s = DetectReservaSchema(cn);

            using (var chk = new MySqlCommand($@"
                SELECT COUNT(1)
                FROM Reservas
                WHERE {s.HabIdCol}=@H
                  AND {s.EstadoCol}<>'Cancelada'
                  AND {s.IdCol}<>@Id
                  AND NOT (@Hasta <= {s.DesdeCol} OR @Desde >= {s.HastaCol});", cn))
            {
                chk.Parameters.AddWithValue("@Id", id);
                chk.Parameters.AddWithValue("@H", habitacionId);
                chk.Parameters.AddWithValue("@Desde", desde);
                chk.Parameters.AddWithValue("@Hasta", hasta);

                if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                    return -100;
            }

            using var cmd = new MySqlCommand($@"
            UPDATE Reservas
               SET {s.HabIdCol}=@H,
                   {s.DesdeCol}=@Desde,
                   {s.HastaCol}=@Hasta,
                   {s.TotalCol}=@Precio
             WHERE {s.IdCol}=@Id AND {s.UserIdCol}=@U;", cn);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@U", usuarioId);
            cmd.Parameters.AddWithValue("@H", habitacionId);
            cmd.Parameters.AddWithValue("@Desde", desde);
            cmd.Parameters.AddWithValue("@Hasta", hasta);
            cmd.Parameters.AddWithValue("@Precio", precioTotal);

            return cmd.ExecuteNonQuery();
        }

        // ===================================================================
        // REPORTE MENSUAL (Nico) – ajustado a nombres dinámicos
        // ===================================================================
        public List<ReservaReporteDto> ObtenerReporteMensual(
            int anio, int mes, bool incluirCanceladas, bool usarStoredProcedure = false)
        {
            var lista = new List<ReservaReporteDto>();
            using var cn = CreateConnection();
            cn.Open();
            var s = DetectReservaSchema(cn);

            using var cmd = new MySqlCommand { Connection = cn };

            if (usarStoredProcedure)
            {
                cmd.CommandText = "sp_reservas_reporte_mes";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_anio", anio);
                cmd.Parameters.AddWithValue("@p_mes", mes);
                cmd.Parameters.AddWithValue("@p_incluir_canceladas", incluirCanceladas ? 1 : 0);
            }
            else
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
                SELECT
                    r.{s.IdCol} AS IdReserva,
                    {(s.HuespedCol != null ? $"r.{s.HuespedCol}" : "''")} AS Huesped,

                    (SELECT h.Numero FROM habitaciones h WHERE h.Id = r.{s.HabIdCol} LIMIT 1) AS HabitacionNumero,
                    (SELECT COALESCE(th.Nombre,'Standard')
                       FROM habitaciones h
                       LEFT JOIN tipo_habitacion th ON th.Id = h.TipoId
                      WHERE h.Id = r.{s.HabIdCol}
                      LIMIT 1) AS HabitacionTipo,

                    r.{s.DesdeCol} AS FechaDesde,
                    r.{s.HastaCol} AS FechaHasta,
                    DATEDIFF(r.{s.HastaCol}, r.{s.DesdeCol}) AS Noches,

                    r.{s.TotalCol} AS Total,
                    r.{s.EstadoCol} AS Estado
                FROM reservas r
                WHERE YEAR(r.{s.DesdeCol}) = @anio
                  AND MONTH(r.{s.DesdeCol}) = @mes
                  AND (@incl = 1 OR r.{s.EstadoCol} <> 'Cancelada')
                ORDER BY r.{s.DesdeCol}, r.{s.IdCol};";

                cmd.Parameters.AddWithValue("@anio", anio);
                cmd.Parameters.AddWithValue("@mes", mes);
                cmd.Parameters.AddWithValue("@incl", incluirCanceladas ? 1 : 0);
            }

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new ReservaReporteDto
                {
                    IdReserva = Convert.ToInt32(rd["IdReserva"]),
                    Huesped = rd["Huesped"]?.ToString() ?? "",
                    HabitacionNumero = rd["HabitacionNumero"]?.ToString() ?? "",
                    HabitacionTipo = rd["HabitacionTipo"]?.ToString() ?? "",
                    FechaDesde = Convert.ToDateTime(rd["FechaDesde"]),
                    FechaHasta = Convert.ToDateTime(rd["FechaHasta"]),
                    Noches = Convert.ToInt32(rd["Noches"]),
                    Total = Convert.ToDecimal(rd["Total"], CultureInfo.InvariantCulture),
                    Estado = rd["Estado"]?.ToString() ?? "Confirmada"
                });
            }
            return lista;
        }

        // ===================================================================
        // BLOQUE DESTINOS / HABITACIONES (Tsho)
        // ===================================================================
        public IEnumerable<(int Id, string Nombre, string? Descripcion)> ListarDestinosActivos()
        {
            // Si en el futuro agregan tabla destinos, acá se consulta.
            yield return (1, "Hotel Aloha", "Sede principal");
        }

        public IEnumerable<HabitacionDTO> ListarHabitacionesPorDestino(int idDestinoIgnorado)
        {
            const string sql = @"
                SELECT  Id, Numero, Capacidad, PrecioBase,
                        COALESCE(Descripcion,'') Descripcion,
                        COALESCE(Estado,'Disponible') Estado
                FROM habitaciones
                ORDER BY Numero;";

            using var cn = CreateConnection();
            using var cmd = new MySqlCommand(sql, cn);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                yield return new HabitacionDTO
                {
                    Id = rd.GetInt32("Id"),
                    Nombre = $"Hab. {rd.GetInt32("Numero")}",
                    Capacidad = rd.IsDBNull(rd.GetOrdinal("Capacidad")) ? 2 : rd.GetInt32("Capacidad"),
                    PrecioNoche = Convert.ToDecimal(rd["PrecioBase"], CultureInfo.InvariantCulture),
                    Descripcion = rd["Descripcion"]?.ToString(),
                    Estado = rd["Estado"]?.ToString()
                };
            }
        }

        // ===================================================================
        // BLOQUE SERVICIOS / RESERVA CON SERVICIOS (Tsho)
        // ===================================================================
        public IEnumerable<ServicioDTO> ListarServiciosActivos()
        {
            const string sql = @"
                SELECT Id, Nombre, Descripcion, Precio, PrecioTipo, Activo
                FROM servicios
                WHERE Activo = 1
                ORDER BY Nombre;";

            using var cn = CreateConnection();
            using var cmd = new MySqlCommand(sql, cn);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var tipoRaw = rd.IsDBNull(rd.GetOrdinal("PrecioTipo"))
                                ? null
                                : rd.GetString("PrecioTipo");

                var tipo = string.IsNullOrWhiteSpace(tipoRaw) ? "PorEstadia" : tipoRaw.Trim();
                if (!string.Equals(tipo, "PorNoche", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(tipo, "PorEstadia", StringComparison.OrdinalIgnoreCase))
                {
                    tipo = "PorEstadia";
                }

                yield return new ServicioDTO
                {
                    Id = rd.GetInt32("Id"),
                    Nombre = rd.GetString("Nombre"),
                    Descripcion = rd.IsDBNull(rd.GetOrdinal("Descripcion")) ? null : rd.GetString("Descripcion"),
                    Precio = Convert.ToDecimal(rd["Precio"], CultureInfo.InvariantCulture),
                    PrecioTipo = tipo,
                    Activo = rd.GetBoolean("Activo")
                };
            }
        }

        public int CrearReservaConServicios(
            int idUsuario,
            int idHabitacion,
            string huesped,
            DateTime fechaDesde,
            DateTime fechaHasta,
            IEnumerable<(int IdServicio, int Cantidad)> servicios)
        {
            if (fechaHasta.Date <= fechaDesde.Date)
                throw new ArgumentException("El rango de fechas es inválido.");

            using var cn = CreateConnection();
            cn.Open();
            var s = DetectReservaSchema(cn);
            using var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);

            using (var check = new MySqlCommand($@"
                SELECT COUNT(*)
                FROM reservas r
                WHERE r.{s.HabIdCol} = @hab
                  AND r.{s.EstadoCol} <> 'Cancelada'
                  AND (@desde < r.{s.HastaCol}) AND (@hasta > r.{s.DesdeCol});", cn, tx))
            {
                check.Parameters.AddWithValue("@hab", idHabitacion);
                check.Parameters.AddWithValue("@desde", fechaDesde.Date);
                check.Parameters.AddWithValue("@hasta", fechaHasta.Date);
                if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                    throw new InvalidOperationException("La habitación no está disponible en ese rango.");
            }

            // Precio base
            decimal precioBase;
            using (var p = new MySqlCommand(@"SELECT PrecioBase FROM habitaciones WHERE Id = @hab;", cn, tx))
            {
                p.Parameters.AddWithValue("@hab", idHabitacion);
                var obj = p.ExecuteScalar() ?? throw new InvalidOperationException("Habitación inexistente.");
                precioBase = Convert.ToDecimal(obj, CultureInfo.InvariantCulture);
            }

            var noches = (int)(fechaHasta.Date - fechaDesde.Date).TotalDays;
            if (noches <= 0) throw new InvalidOperationException("Checkout debe ser posterior al checkin.");

            decimal totalServicios = 0m;
            var serviciosList = servicios?.ToList() ?? new List<(int, int)>();
            if (serviciosList.Count > 0)
            {
                var ids = string.Join(",", serviciosList.Select(sv => sv.IdServicio));
                var info = new Dictionary<int, (decimal Precio, string Tipo)>();

                using (var c = new MySqlCommand($"SELECT Id, Precio, PrecioTipo FROM servicios WHERE Id IN ({ids})", cn, tx))
                using (var rd = c.ExecuteReader())
                {
                    while (rd.Read())
                        info[rd.GetInt32("Id")] = (
                            Convert.ToDecimal(rd["Precio"], CultureInfo.InvariantCulture),
                            rd.GetString("PrecioTipo")
                        );
                }

                foreach (var sLine in serviciosList)
                {
                    if (!info.TryGetValue(sLine.IdServicio, out var i)) continue;

                    var tipo = (i.Tipo ?? "PorEstadia").Trim();
                    if (!string.Equals(tipo, "PorNoche", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(tipo, "PorEstadia", StringComparison.OrdinalIgnoreCase))
                    {
                        tipo = "PorEstadia";
                    }

                    var mult = string.Equals(tipo, "PorNoche", StringComparison.OrdinalIgnoreCase) ? noches : 1;
                    totalServicios += i.Precio * sLine.Cantidad * mult;
                }
            }

            var totalHabitacion = precioBase * noches;
            var totalReserva = totalHabitacion + totalServicios;

            int idReserva;
            {
                var colNames = new List<string> { s.UserIdCol, s.HabIdCol, s.DesdeCol, s.HastaCol, s.TotalCol, s.EstadoCol };
                var parNames = new List<string> { "@uid", "@hab", "@desde", "@hasta", "@total", "'Confirmada'" };
                if (s.HuespedCol != null) { colNames.Insert(2, s.HuespedCol); parNames.Insert(2, "@huesped"); }

                string insSql = $@"
                    INSERT INTO reservas ({string.Join(",", colNames)})
                    VALUES ({string.Join(",", parNames)});
                    SELECT LAST_INSERT_ID();";

                using var ins = new MySqlCommand(insSql, cn, tx);
                ins.Parameters.AddWithValue("@uid", idUsuario);
                ins.Parameters.AddWithValue("@hab", idHabitacion);
                ins.Parameters.AddWithValue("@desde", fechaDesde.Date);
                ins.Parameters.AddWithValue("@hasta", fechaHasta.Date);
                ins.Parameters.AddWithValue("@total", totalReserva);
                if (s.HuespedCol != null) ins.Parameters.AddWithValue("@huesped", huesped ?? "");

                idReserva = Convert.ToInt32(ins.ExecuteScalar());
            }

            foreach (var sLine in serviciosList)
            {
                using var line = new MySqlCommand(@"
                    INSERT INTO reserva_servicios (IdReserva, IdServicio, Cantidad, PrecioUnit, PrecioTipo)
                    SELECT @rid, sv.Id, @cant, sv.Precio, sv.PrecioTipo
                    FROM servicios sv
                    WHERE sv.Id = @sid;", cn, tx);
                line.Parameters.AddWithValue("@rid", idReserva);
                line.Parameters.AddWithValue("@sid", sLine.IdServicio);
                line.Parameters.AddWithValue("@cant", sLine.Cantidad);
                line.ExecuteNonQuery();
            }

            tx.Commit();
            return idReserva;
        }

        public ReservaDetalleParaPdf? ObtenerReservaParaPdf(int idReserva, int idUsuario)
        {
            using var cn = CreateConnection();
            cn.Open();
            var s = DetectReservaSchema(cn);

            string sql = $@"
                SELECT r.{s.IdCol} Id, r.{s.DesdeCol} FechaDesde, r.{s.HastaCol} FechaHasta, r.{s.EstadoCol} Estado, r.{s.TotalCol} Total,
                       {(s.HuespedCol != null ? $"r.{s.HuespedCol}" : "''")} Huesped,
                       h.Numero, h.Capacidad, h.PrecioBase, COALESCE(h.Descripcion,'') DescripcionHab
                FROM reservas r
                JOIN habitaciones h ON h.Id = r.{s.HabIdCol}
                WHERE r.{s.IdCol} = @id AND r.{s.UserIdCol} = @uid;";

            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", idReserva);
            cmd.Parameters.AddWithValue("@uid", idUsuario);

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            var desde = Convert.ToDateTime(rd["FechaDesde"]).Date;
            var hasta = Convert.ToDateTime(rd["FechaHasta"]).Date;

            return new ReservaDetalleParaPdf
            {
                Id = Convert.ToInt32(rd["Id"]),
                Desde = desde,
                Hasta = hasta,
                Estado = rd["Estado"]?.ToString() ?? "Confirmada",
                Creada = DateTime.Now,
                Habitacion = $"Hab. {Convert.ToInt32(rd["Numero"])}",
                Capacidad = rd.IsDBNull(rd.GetOrdinal("Capacidad")) ? 2 : Convert.ToInt32(rd["Capacidad"]),
                PrecioNoche = Convert.ToDecimal(rd["PrecioBase"], CultureInfo.InvariantCulture),
                Destino = "Hotel Aloha",
                DestinoDescripcion = rd["DescripcionHab"]?.ToString(),
                Huesped = rd["Huesped"]?.ToString(),
                Total = Convert.ToDecimal(rd["Total"], CultureInfo.InvariantCulture),
            };
        }

        public (ReservaDetalleParaPdf Cabecera, List<ReservaServicioLineaDTO> Lineas)
            ObtenerReservaParaPdfConServicios(int idReserva, int idUsuario)
        {
            var cab = ObtenerReservaParaPdf(idReserva, idUsuario)
                      ?? throw new InvalidOperationException("Reserva inexistente.");

            var lineas = new List<ReservaServicioLineaDTO>();
            const string sql = @"
                SELECT rs.IdServicio, s.Nombre, rs.Cantidad, rs.PrecioUnit, rs.PrecioTipo
                FROM reserva_servicios rs
                JOIN servicios s ON s.Id = rs.IdServicio
                WHERE rs.IdReserva = @rid";

            using var cn = CreateConnection();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@rid", idReserva);
            cn.Open();
            using var rd = cmd.ExecuteReader();

            var noches = (int)(cab.Hasta - cab.Desde).TotalDays;
            while (rd.Read())
            {
                var tipoRaw = rd["PrecioTipo"]?.ToString();
                var tipo = string.IsNullOrWhiteSpace(tipoRaw) ? "PorEstadia" : tipoRaw.Trim();
                if (!string.Equals(tipo, "PorNoche", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(tipo, "PorEstadia", StringComparison.OrdinalIgnoreCase))
                {
                    tipo = "PorEstadia";
                }
                var unit = Convert.ToDecimal(rd["PrecioUnit"], CultureInfo.InvariantCulture);
                var cant = Convert.ToInt32(rd["Cantidad"]);
                var mult = string.Equals(tipo, "PorNoche", StringComparison.OrdinalIgnoreCase) ? noches : 1;

                lineas.Add(new ReservaServicioLineaDTO
                {
                    IdServicio = Convert.ToInt32(rd["IdServicio"]),
                    Nombre = rd["Nombre"]?.ToString() ?? "",
                    Cantidad = cant,
                    PrecioUnit = unit,
                    PrecioTipo = tipo,
                    Subtotal = unit * cant * mult
                });
            }
            return (cab, lineas);
        }
    }
}
