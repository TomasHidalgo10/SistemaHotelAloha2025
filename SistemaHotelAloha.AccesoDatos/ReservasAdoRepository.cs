
using MySql.Data.MySqlClient;
using SistemaHotelAloha.AccesoDatos.Infra;
using SistemaHotelAloha.AccesoDatos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.AccesoDatos
{
    public class ReservasAdoRepository
    {
        // DTO para la grilla
        public sealed record ReservaDto(
            int Id,
            int HabitacionId,
            string HabitacionLabel,
            DateTime Desde,
            DateTime Hasta,
            decimal PrecioTotal,
            string Estado
        );

        // --------------------------------------------------------------------
        //  HABITACIONES DISPONIBLES entre dos fechas
        //  Devuelve (Id, Label, PrecioBase)
        // --------------------------------------------------------------------
        public List<(int Id, string Label, decimal Precio)> GetHabitacionesDisponibles(DateTime desde, DateTime hasta)
        {
            using var cn = MySqlConnectionFactory.Create();
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
                var precio = Convert.ToDecimal(rd["PrecioBase"]);
                list.Add((id, label, precio));
            }
            return list;
        }

        // --------------------------------------------------------------------
        //  CREAR reserva (valida solapamiento)
        //  Devuelve: Id nuevo o -100 si hay solapamiento
        // --------------------------------------------------------------------
        public int Crear(int usuarioId, int habitacionId, DateTime desde, DateTime hasta, decimal precioTotal)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();

            // Chequeo de solapamiento en esa habitación
            using (var chk = new MySqlCommand(@"
            SELECT COUNT(1)
            FROM Reservas
            WHERE HabitacionId=@H
              AND Estado<>'Cancelada'
              AND NOT (@Hasta <= FechaDesde OR @Desde >= FechaHasta);", cn))
            {
                chk.Parameters.AddWithValue("@H", habitacionId);
                chk.Parameters.AddWithValue("@Desde", desde);
                chk.Parameters.AddWithValue("@Hasta", hasta);

                if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                    return -100;
            }

            using var cmd = new MySqlCommand(@"
            INSERT INTO Reservas (UsuarioId, HabitacionId, FechaDesde, FechaHasta, Estado, PrecioTotal, CreadaEn)
            VALUES (@U, @H, @Desde, @Hasta, 'Confirmada', @Precio, UTC_TIMESTAMP());
            SELECT LAST_INSERT_ID();", cn);

            cmd.Parameters.AddWithValue("@U", usuarioId);
            cmd.Parameters.AddWithValue("@H", habitacionId);
            cmd.Parameters.AddWithValue("@Desde", desde);
            cmd.Parameters.AddWithValue("@Hasta", hasta);
            cmd.Parameters.AddWithValue("@Precio", precioTotal);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // --------------------------------------------------------------------
        //  LISTAR reservas por usuario (para la grilla)
        // --------------------------------------------------------------------
        public List<ReservaDto> GetByUsuario(int usuarioId)
        {
            using var cn = MySqlConnectionFactory.Create();
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
                    PrecioTotal: Convert.ToDecimal(rd["PrecioTotal"]),
                    Estado: rd["Estado"]?.ToString() ?? "Confirmada"
                ));
            }
            return result;
        }

        // --------------------------------------------------------------------
        //  CANCELAR reserva
        // --------------------------------------------------------------------
        public bool Cancelar(int id, int usuarioId)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
            UPDATE Reservas
            SET Estado='Cancelada'
            WHERE Id=@Id AND UsuarioId=@U;", cn);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@U", usuarioId);

            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public int Actualizar(int id, int usuarioId, int habitacionId, DateTime desde, DateTime hasta, decimal precioTotal)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();

            // Chequeo de solapamiento EXCLUYENDO la misma reserva (Id)
            using (var chk = new MySqlCommand(@"
            SELECT COUNT(1)
            FROM Reservas
            WHERE HabitacionId=@H
              AND Estado<>'Cancelada'
              AND Id<>@Id
              AND NOT (@Hasta <= FechaDesde OR @Desde >= FechaHasta);", cn))
            {
                chk.Parameters.AddWithValue("@Id", id);
                chk.Parameters.AddWithValue("@H", habitacionId);
                chk.Parameters.AddWithValue("@Desde", desde);
                chk.Parameters.AddWithValue("@Hasta", hasta);

                if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                    return -100;
            }

            using var cmd = new MySqlCommand(@"
            UPDATE Reservas
               SET HabitacionId=@H,
                   FechaDesde=@Desde,
                   FechaHasta=@Hasta,
                   PrecioTotal=@Precio
             WHERE Id=@Id AND UsuarioId=@U;", cn);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@U", usuarioId);
            cmd.Parameters.AddWithValue("@H", habitacionId);
            cmd.Parameters.AddWithValue("@Desde", desde);
            cmd.Parameters.AddWithValue("@Hasta", hasta);
            cmd.Parameters.AddWithValue("@Precio", precioTotal);

            return cmd.ExecuteNonQuery();
        }

        // --------------------------------------------------------------------
        //  LISTAR reservas con filtro opcional por estado (Confirmada/Cancelada)
        // --------------------------------------------------------------------
        public List<ReservaDto> GetByUsuario(int usuarioId, string? estado = null)
        {
            using var cn = MySqlConnectionFactory.Create();
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
                    PrecioTotal: Convert.ToDecimal(rd["PrecioTotal"]),
                    Estado: rd["Estado"]?.ToString() ?? "Confirmada"
                ));
            }
            return result;
        }

        // --------------------------------------------------------------------
        //  REPORTE MENSUAL DE RESERVAS (para PDF)
        //  - anio / mes: período a consultar
        //  - incluirCanceladas: si true, incluye Estado='Cancelada'
        //  - usarStoredProcedure: si tenés creado sp_reservas_reporte_mes
        //    (sino pasá usarStoredProcedure: false y usa SQL directo)
        //  Devuelve: List<ReservaReporteDto>
        // --------------------------------------------------------------------
        public List<ReservaReporteDto> ObtenerReporteMensual(
    int anio,
    int mes,
    bool incluirCanceladas,
    bool usarStoredProcedure = false)
        {
            var lista = new List<ReservaReporteDto>();

            using var cn = MySqlConnectionFactory.Create();
            cn.Open();

            using var cmd = new MySqlCommand();
            cmd.Connection = cn;

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
                // --- 1) Leer nombres reales de columnas en 'reservas'
                var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var colCmd = new MySqlCommand(@"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'reservas';", cn))
                using (var rdCols = colCmd.ExecuteReader())
                {
                    while (rdCols.Read())
                        cols.Add(rdCols.GetString(0));
                }

                // Candidatos comunes para la FK a habitaciones
                var habIdCandidates = new[]
                {
                "HabitacionId","IdHabitacion","id_habitacion","habitacion_id",
                "idHabitacion","habitacionId","Id_Habitacion"
                };

                string? habIdCol = habIdCandidates.FirstOrDefault(c => cols.Contains(c));

                // Si no aparece, intentamos descubrirla por FK
                if (habIdCol is null)
                {
                    using var fkCmd = new MySqlCommand(@"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = 'reservas'
                  AND REFERENCED_TABLE_NAME = 'habitaciones'
                LIMIT 1;", cn);
                    var fkCol = fkCmd.ExecuteScalar() as string;
                    if (!string.IsNullOrWhiteSpace(fkCol))
                        habIdCol = fkCol;
                }

                // Si sigue sin encontrarse, lanzamos un error claro
                if (habIdCol is null)
                {
                    throw new Exception("No se pudo detectar la columna FK a 'habitaciones' en la tabla 'reservas'. " +
                                        "Probá renombrar la columna a 'IdHabitacion' o decime el nombre real para ajustar el reporte.");
                }

                // Otras columnas (más tolerantes a nombres)
                string idCol = cols.Contains("Id") ? "Id" : cols.Contains("id") ? "id" : "Id";
                string fdCol = cols.Contains("FechaDesde") ? "FechaDesde" : cols.Contains("fecha_desde") ? "fecha_desde" : "FechaDesde";
                string fhCol = cols.Contains("FechaHasta") ? "FechaHasta" : cols.Contains("fecha_hasta") ? "fecha_hasta" : "FechaHasta";
                string estadoCol = cols.Contains("Estado") ? "Estado" : cols.Contains("estado") ? "estado" : "Estado";
                string totalCol = cols.Contains("PrecioTotal") ? "PrecioTotal"
                                 : (cols.Contains("total") ? "total"
                                 : cols.Contains("Precio") ? "Precio" : "total");

                bool hasHuesped = cols.Contains("Huesped") || cols.Contains("huesped");
                string huespedCol = cols.Contains("Huesped") ? "Huesped" : (cols.Contains("huesped") ? "huesped" : "");

                // --- 2) Armamos SQL. Usamos subconsultas para evitar JOIN si el nombre varía.
                string selectHuesped = hasHuesped ? $"r.{huespedCol}" : "''";

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
                SELECT
                    r.{idCol} AS IdReserva,
                    {selectHuesped} AS Huesped,

                    /* Traemos datos de la habitación sin JOIN (evita 'unknown column' en ON) */
                    (SELECT h.Numero FROM habitaciones h WHERE h.Id = r.{habIdCol} LIMIT 1) AS HabitacionNumero,
                    (SELECT COALESCE(th.Nombre,'Standard')
                       FROM habitaciones h
                       LEFT JOIN tipo_habitacion th ON th.Id = h.TipoId
                      WHERE h.Id = r.{habIdCol}
                      LIMIT 1) AS HabitacionTipo,

                    r.{fdCol} AS FechaDesde,
                    r.{fhCol} AS FechaHasta,
                    DATEDIFF(r.{fhCol}, r.{fdCol}) AS Noches,

                    r.{totalCol} AS Total,
                    r.{estadoCol} AS Estado
                    FROM reservas r
                    WHERE YEAR(r.{fdCol}) = @anio
                      AND MONTH(r.{fdCol}) = @mes
                      AND (@incl = 1 OR r.{estadoCol} <> 'Cancelada')
                    ORDER BY r.{fdCol}, r.{idCol};";

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
                    Total = Convert.ToDecimal(rd["Total"]),
                    Estado = rd["Estado"]?.ToString() ?? "Confirmada"
                });
            }

            return lista;
        }
    }
}