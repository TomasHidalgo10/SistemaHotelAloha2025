

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using MySql.Data.MySqlClient;
using SistemaHotelAloha.AccesoDatos.Models;

namespace SistemaHotelAloha.AccesoDatos
{
    public class ReservasAdoRepository
    {
        private readonly string _connStr;

        public ReservasAdoRepository(string connectionString)
        {
            _connStr = connectionString ?? "";
            if (string.IsNullOrWhiteSpace(_connStr))
                Console.WriteLine("⚠️ DefaultConnection vacío. Configurá appsettings/variables de entorno del proyecto Web.");
        }

        private MySqlConnection Conn() => new MySqlConnection(_connStr);

        
        public IEnumerable<(int Id, string Nombre, string? Descripcion)> ListarDestinosActivos()
        {
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

            using var cn = Conn();
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

       
        public bool HabitacionDisponible(int idHabitacion, DateTime fechaDesde, DateTime fechaHasta)
        {
            if (fechaHasta.Date <= fechaDesde.Date)
                throw new ArgumentException("El rango de fechas es inválido.");

            const string sql = @"
                SELECT NOT EXISTS (
                    SELECT 1
                    FROM reservas r
                    WHERE r.IdHabitacion = @hab
                      AND r.Estado <> 'Cancelada'
                      AND (@desde < r.FechaHasta) AND (@hasta > r.FechaDesde)
                );";

            using var cn = Conn();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@hab", idHabitacion);
            cmd.Parameters.AddWithValue("@desde", fechaDesde.Date);
            cmd.Parameters.AddWithValue("@hasta", fechaHasta.Date);
            cn.Open();
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        
        public int CrearReserva(int idUsuario, int idHabitacion, string huesped, DateTime fechaDesde, DateTime fechaHasta)
        {
            if (fechaHasta.Date <= fechaDesde.Date)
                throw new ArgumentException("El rango de fechas es inválido.");

            using var cn = Conn();
            cn.Open();
            using var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);

            using (var check = new MySqlCommand(@"
                SELECT COUNT(*)
                FROM reservas r
                WHERE r.IdHabitacion = @hab
                  AND r.Estado <> 'Cancelada'
                  AND (@desde < r.FechaHasta) AND (@hasta > r.FechaDesde);", cn, tx))
            {
                check.Parameters.AddWithValue("@hab", idHabitacion);
                check.Parameters.AddWithValue("@desde", fechaDesde.Date);
                check.Parameters.AddWithValue("@hasta", fechaHasta.Date);
                if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                    throw new InvalidOperationException("La habitación no está disponible en ese rango.");
            }

            decimal precioBase;
            using (var p = new MySqlCommand(@"SELECT PrecioBase FROM habitaciones WHERE Id = @hab;", cn, tx))
            {
                p.Parameters.AddWithValue("@hab", idHabitacion);
                var obj = p.ExecuteScalar() ?? throw new InvalidOperationException("Habitación inexistente.");
                precioBase = Convert.ToDecimal(obj, CultureInfo.InvariantCulture);
            }

            var noches = (int)(fechaHasta.Date - fechaDesde.Date).TotalDays;
            var total = precioBase * noches;

            using (var ins = new MySqlCommand(@"
                INSERT INTO reservas (IdUsuario, IdHabitacion, Huesped, FechaDesde, FechaHasta, Total, Estado)
                VALUES (@uid, @hab, @huesped, @desde, @hasta, @total, 'Confirmada');
                SELECT LAST_INSERT_ID();", cn, tx))
            {
                ins.Parameters.AddWithValue("@uid", idUsuario);
                ins.Parameters.AddWithValue("@hab", idHabitacion);
                ins.Parameters.AddWithValue("@huesped", huesped ?? "");
                ins.Parameters.AddWithValue("@desde", fechaDesde.Date);
                ins.Parameters.AddWithValue("@hasta", fechaHasta.Date);
                ins.Parameters.AddWithValue("@total", total);

                var newId = Convert.ToInt32(ins.ExecuteScalar());
                tx.Commit();
                return newId;
            }
        }

        
        public IEnumerable<ReservaDTO> ListarReservasDeUsuario(int idUsuario)
        {
            const string sql = @"
                SELECT r.Id, r.FechaDesde, r.FechaHasta, r.Estado, r.Total,
                       h.Numero, h.PrecioBase
                FROM reservas r
                JOIN habitaciones h ON h.Id = r.IdHabitacion
                WHERE r.IdUsuario = @uid
                ORDER BY r.FechaDesde DESC;";

            using var cn = Conn();
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

        
        public bool CancelarReserva(int idReserva, int idUsuario)
        {
            const string sql = @"
                UPDATE reservas
                   SET Estado = 'Cancelada'
                 WHERE Id = @id
                   AND IdUsuario = @uid
                   AND Estado <> 'Cancelada';";

            using var cn = Conn();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", idReserva);
            cmd.Parameters.AddWithValue("@uid", idUsuario);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        
        public ReservaDetalleParaPdf? ObtenerReservaParaPdf(int idReserva, int idUsuario)
        {
            const string sql = @"
                SELECT r.Id, r.FechaDesde, r.FechaHasta, r.Estado, r.Total, r.Huesped,
                       h.Numero, h.Capacidad, h.PrecioBase, COALESCE(h.Descripcion,'') DescripcionHab
                FROM reservas r
                JOIN habitaciones h ON h.Id = r.IdHabitacion
                WHERE r.Id = @id AND r.IdUsuario = @uid;";

            using var cn = Conn();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", idReserva);
            cmd.Parameters.AddWithValue("@uid", idUsuario);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            var desde = rd.GetDateTime("FechaDesde").Date;
            var hasta = rd.GetDateTime("FechaHasta").Date;

            return new ReservaDetalleParaPdf
            {
                Id = rd.GetInt32("Id"),
                Desde = desde,
                Hasta = hasta,
                Estado = rd.GetString("Estado"),
                Creada = DateTime.Now,
                Habitacion = $"Hab. {rd.GetInt32("Numero")}",
                Capacidad = rd.IsDBNull(rd.GetOrdinal("Capacidad")) ? 2 : rd.GetInt32("Capacidad"),
                PrecioNoche = Convert.ToDecimal(rd["PrecioBase"], CultureInfo.InvariantCulture),
                Destino = "Hotel Aloha",
                DestinoDescripcion = rd["DescripcionHab"]?.ToString(),
                Huesped = rd["Huesped"]?.ToString(),
                Total = Convert.ToDecimal(rd["Total"], CultureInfo.InvariantCulture),
            };
        }


        public IEnumerable<ServicioDTO> ListarServiciosActivos()
        {
            const string sql = @"
        SELECT Id, Nombre, Descripcion, Precio, PrecioTipo, Activo
        FROM servicios
        WHERE Activo = 1
        ORDER BY Nombre;";

            using var cn = Conn();
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

            using var cn = Conn();
            cn.Open();
            using var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);

            using (var check = new MySqlCommand(@"
                SELECT COUNT(*)
                FROM reservas r
                WHERE r.IdHabitacion = @hab
                  AND r.Estado <> 'Cancelada'
                  AND (@desde < r.FechaHasta) AND (@hasta > r.FechaDesde);", cn, tx))
            {
                check.Parameters.AddWithValue("@hab", idHabitacion);
                check.Parameters.AddWithValue("@desde", fechaDesde.Date);
                check.Parameters.AddWithValue("@hasta", fechaHasta.Date);
                if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                    throw new InvalidOperationException("La habitación no está disponible en ese rango.");
            }

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
                var ids = string.Join(",", serviciosList.Select(s => s.IdServicio));
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

                foreach (var s in serviciosList)
                {
                    if (!info.TryGetValue(s.IdServicio, out var i)) continue;

                    var tipo = (i.Tipo ?? "PorEstadia").Trim();
                    if (!string.Equals(tipo, "PorNoche", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(tipo, "PorEstadia", StringComparison.OrdinalIgnoreCase))
                    {
                        tipo = "PorEstadia";
                    }

                    var mult = string.Equals(tipo, "PorNoche", StringComparison.OrdinalIgnoreCase) ? noches : 1;
                    totalServicios += i.Precio * s.Cantidad * mult;
                }
            }

            var totalHabitacion = precioBase * noches;
            var totalReserva = totalHabitacion + totalServicios;

            int idReserva;
            using (var ins = new MySqlCommand(@"
                INSERT INTO reservas (IdUsuario, IdHabitacion, Huesped, FechaDesde, FechaHasta, Total, Estado)
                VALUES (@uid, @hab, @huesped, @desde, @hasta, @total, 'Confirmada');
                SELECT LAST_INSERT_ID();", cn, tx))
            {
                ins.Parameters.AddWithValue("@uid", idUsuario);
                ins.Parameters.AddWithValue("@hab", idHabitacion);
                ins.Parameters.AddWithValue("@huesped", huesped ?? "");
                ins.Parameters.AddWithValue("@desde", fechaDesde.Date);
                ins.Parameters.AddWithValue("@hasta", fechaHasta.Date);
                ins.Parameters.AddWithValue("@total", totalReserva);

                idReserva = Convert.ToInt32(ins.ExecuteScalar());
            }

            foreach (var s in serviciosList)
            {
                using var line = new MySqlCommand(@"
                    INSERT INTO reserva_servicios (IdReserva, IdServicio, Cantidad, PrecioUnit, PrecioTipo)
                    SELECT @rid, sv.Id, @cant, sv.Precio, sv.PrecioTipo
                    FROM servicios sv
                    WHERE sv.Id = @sid;", cn, tx);
                line.Parameters.AddWithValue("@rid", idReserva);
                line.Parameters.AddWithValue("@sid", s.IdServicio);
                line.Parameters.AddWithValue("@cant", s.Cantidad);
                line.ExecuteNonQuery();
            }

            tx.Commit();
            return idReserva;
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

            using var cn = Conn();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@rid", idReserva);
            cn.Open();
            using var rd = cmd.ExecuteReader();

            var noches = (int)(cab.Hasta - cab.Desde).TotalDays;
            while (rd.Read())
            {
                var tipoRaw = rd.GetString("PrecioTipo");
                var tipo = string.IsNullOrWhiteSpace(tipoRaw) ? "PorEstadia" : tipoRaw.Trim();
                if (!string.Equals(tipo, "PorNoche", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(tipo, "PorEstadia", StringComparison.OrdinalIgnoreCase))
                {
                    tipo = "PorEstadia";
                }
                var unit = Convert.ToDecimal(rd["PrecioUnit"], CultureInfo.InvariantCulture);
                var cant = rd.GetInt32("Cantidad");
                var mult = string.Equals(tipo, "PorNoche", StringComparison.OrdinalIgnoreCase) ? noches : 1;

                lineas.Add(new ReservaServicioLineaDTO
                {
                    IdServicio = rd.GetInt32("IdServicio"),
                    Nombre = rd.GetString("Nombre"),
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
