using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SistemaHotelAloha.AccesoDatos
{
    /// <summary>
    /// ADO.NET Repo unificado para esquema:
    /// usuarios(Id,Nombre,Apellido,Email,Contrasena,Telefono,FechaRegistro,Activo,role_id)
    /// clientes(Id,IdUsuario, Dni, Direccion, Ciudad)
    /// tipo_habitacion(Id,Nombre,Descripcion)
    /// estado_habitacion(Id,Nombre)
    /// habitaciones(Id,Numero,TipoId,EstadoId,PrecioBase,Descripcion,Foto)
    /// estado_reserva(Id,Nombre)
    /// reservas(Id,IdUsuario,IdHabitacion,FechaDesde,FechaHasta,Total,EstadoId)
    /// reserva_servicio(Id,ReservaId,ServicioId,Precio,Cantidad,Total)
    /// servicios_adicionales(Id,Nombre,Precio,Descripcion,Activo)
    /// </summary>
    public class ReservasAdoRepository
    {
        private readonly string _connString;

        public ReservasAdoRepository(string connString)
        {
            _connString = connString ?? throw new ArgumentNullException(nameof(connString));
        }

        // ===================== DTOs usados por distintas páginas =====================
        public record TipoHabDto(int Id, string Nombre);

        public record HabitacionDispDto(
            int Id,
            int Numero,
            string Tipo,
            decimal Precio,
            string? Descripcion,
            string? Foto
        );

        // Para compatibilidad con DestinoDetalle.razor
        public record HabitacionDto(
            int Id,
            string Nombre,      // Ej: "Doble Nº 102"
            string Tipo,
            decimal Precio,
            string? Foto,
            string? Descripcion
        );

        // Para compatibilidad con MisReservas.razor
        public record ReservasListaDto(
            int Id,
            string Habitacion,  // Ej: "Suite Nº 104"
            DateTime FechaDesde,
            DateTime FechaHasta,
            decimal Total,
            string Estado
        );

        // ======== DTOs para PDF ========
        public record PdfCabecera(
            int IdReserva,
            string Cliente,
            string Habitacion,
            DateTime FechaDesde,
            DateTime FechaHasta,
            decimal Total
        );

        public record PdfLinea(
            string Concepto,
            decimal Precio,
            int Cantidad,
            decimal Subtotal
        );

        // ===================== Tipos de Habitación (para combo) =====================
        public async Task<List<TipoHabDto>> ObtenerTiposHabitacionAsync()
        {
            var lista = new List<TipoHabDto>();
            await using var cn = new MySqlConnection(_connString);
            await cn.OpenAsync();

            const string sql = @"SELECT Id, Nombre
                                 FROM tipo_habitacion
                                 ORDER BY Id;";

            await using var cmd = new MySqlCommand(sql, cn);
            await using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                lista.Add(new TipoHabDto(
                    rd.GetInt32("Id"),
                    rd.GetString("Nombre")
                ));
            }
            return lista;
        }

        // ===================== Buscar disponibles por tipo y rango (nueva UI) =====================
        public async Task<List<HabitacionDispDto>> BuscarDisponiblesPorTipoAsync(int tipoId, DateTime desde, DateTime hasta)
        {
            var list = new List<HabitacionDispDto>();
            await using var cn = new MySqlConnection(_connString);
            await cn.OpenAsync();

            const string sql = @"
SELECT  h.Id,
        h.Numero,
        th.Nombre      AS Tipo,
        h.PrecioBase   AS Precio,
        h.Descripcion,
        h.Foto
FROM habitaciones h
JOIN tipo_habitacion th ON th.Id = h.TipoId
WHERE h.TipoId = @tipo
  AND h.EstadoId = 1
  AND NOT EXISTS (
      SELECT 1
      FROM reservas r
      WHERE r.IdHabitacion = h.Id
        AND r.EstadoId <> 3              -- 3 = Cancelada
        AND (@desde < r.FechaHasta)      -- solape
        AND (@hasta  > r.FechaDesde)
  )
ORDER BY h.Numero;";

            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@tipo", tipoId);
            cmd.Parameters.AddWithValue("@desde", desde.Date);
            cmd.Parameters.AddWithValue("@hasta", hasta.Date);

            await using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new HabitacionDispDto(
                    rd.GetInt32("Id"),
                    rd.GetInt32("Numero"),
                    rd.GetString("Tipo"),
                    rd.GetDecimal("Precio"),
                    rd["Descripcion"] as string,
                    rd["Foto"] as string
                ));
            }
            return list;
        }

        // ===================== Crear Reserva (usado por flujo de reserva) =====================
        public async Task<int> CrearReserva(int idUsuario, int idCliente, int idHabitacion, DateTime desde, DateTime hasta)
        {
            await using var cn = new MySqlConnection(_connString);
            await cn.OpenAsync();

            decimal precioBase;
            await using (var cmdPrecio = new MySqlCommand(
                "SELECT PrecioBase FROM habitaciones WHERE Id=@id;", cn))
            {
                cmdPrecio.Parameters.AddWithValue("@id", idHabitacion);
                var o = await cmdPrecio.ExecuteScalarAsync();
                if (o is null) throw new Exception("Habitación inexistente.");
                precioBase = Convert.ToDecimal(o);
            }

            var noches = (hasta.Date - desde.Date).Days;
            if (noches <= 0) throw new Exception("Rango de fechas inválido.");
            var total = precioBase * noches;

            const string sql = @"
INSERT INTO reservas (IdUsuario, IdHabitacion, FechaDesde, FechaHasta, Total, EstadoId)
VALUES (@u, @h, @d, @t, @total, 2);    -- 2 = Confirmada
SELECT LAST_INSERT_ID();";

            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@u", idUsuario);
            cmd.Parameters.AddWithValue("@h", idHabitacion);
            cmd.Parameters.AddWithValue("@d", desde.Date);
            cmd.Parameters.AddWithValue("@t", hasta.Date);
            cmd.Parameters.AddWithValue("@total", total);

            var id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return id;
        }

        // ===================== Compat: Lista de reservas de un usuario (MisReservas) =====================
        public async Task<List<ReservasListaDto>> ListarReservasDelUsuario(int idUsuario)
        {
            var list = new List<ReservasListaDto>();
            await using var cn = new MySqlConnection(_connString);
            await cn.OpenAsync();

            const string sql = @"
SELECT  r.Id,
        CONCAT(th.Nombre, ' Nº ', h.Numero) AS Habitacion,
        r.FechaDesde,
        r.FechaHasta,
        r.Total,
        er.Nombre AS Estado
FROM reservas r
JOIN habitaciones h     ON h.Id  = r.IdHabitacion
JOIN tipo_habitacion th ON th.Id = h.TipoId
JOIN estado_reserva er  ON er.Id = r.EstadoId
WHERE r.IdUsuario = @u
ORDER BY r.FechaDesde DESC, r.Id DESC;";

            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@u", idUsuario);

            await using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new ReservasListaDto(
                    rd.GetInt32("Id"),
                    rd.GetString("Habitacion"),
                    rd.GetDateTime("FechaDesde"),
                    rd.GetDateTime("FechaHasta"),
                    rd.GetDecimal("Total"),
                    rd.GetString("Estado")
                ));
            }
            return list;
        }

        // ===================== Compat: ListarHabitacionesPorDestino (Destino = nombre del tipo) =====================
        public async Task<List<HabitacionDto>> ListarHabitacionesPorDestino(string destino)
        {
            var list = new List<HabitacionDto>();
            await using var cn = new MySqlConnection(_connString);
            await cn.OpenAsync();

            // Si 'destino' viene vacío, devuelvo todas las habitaciones.
            const string sql = @"
SELECT  h.Id,
        CONCAT(th.Nombre, ' Nº ', h.Numero) AS Nombre,
        th.Nombre AS Tipo,
        h.PrecioBase AS Precio,
        h.Foto,
        h.Descripcion
FROM habitaciones h
JOIN tipo_habitacion th ON th.Id = h.TipoId
WHERE (@destino = '' OR LOWER(th.Nombre) LIKE LOWER(CONCAT('%', @destino, '%')))
ORDER BY th.Id, h.Numero;";

            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@destino", (destino ?? string.Empty).Trim());

            await using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new HabitacionDto(
                    rd.GetInt32("Id"),
                    rd.GetString("Nombre"),
                    rd.GetString("Tipo"),
                    rd.GetDecimal("Precio"),
                    rd["Foto"] as string,
                    rd["Descripcion"] as string
                ));
            }
            return list;
        }

        // ===================== Compat: Disponibilidad de 1 habitación =====================
        public async Task<bool> HabitacionDisponibleAsync(int idHabitacion, DateTime desde, DateTime hasta)
        {
            await using var cn = new MySqlConnection(_connString);
            await cn.OpenAsync();

            const string sql = @"
SELECT NOT EXISTS (
    SELECT 1
    FROM reservas r
    WHERE r.IdHabitacion = @h
      AND r.EstadoId <> 3
      AND (@desde < r.FechaHasta)
      AND (@hasta  > r.FechaDesde)
) AS Libre;";

            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@h", idHabitacion);
            cmd.Parameters.AddWithValue("@desde", desde.Date);
            cmd.Parameters.AddWithValue("@hasta", hasta.Date);

            var o = await cmd.ExecuteScalarAsync();
            return Convert.ToBoolean(o);
        }

        // ===================== Compat: existe usuario por email =====================
        public async Task<bool> ExisteUsuarioAsync(string email)
        {
            await using var cn = new MySqlConnection(_connString);
            await cn.OpenAsync();

            const string sql = @"SELECT 1 FROM usuarios WHERE LOWER(TRIM(Email)) = LOWER(TRIM(@e)) LIMIT 1;";
            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@e", email);

            var o = await cmd.ExecuteScalarAsync();
            return o != null;
        }

        // ===================== Compat: buscar Id usuario por email =====================
        public async Task<int?> BuscarUsuarioIdPorEmailAsync(string email)
        {
            await using var cn = new MySqlConnection(_connString);
            await cn.OpenAsync();

            const string sql = @"SELECT Id FROM usuarios WHERE LOWER(TRIM(Email)) = LOWER(TRIM(@e)) LIMIT 1;";
            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@e", email);

            var o = await cmd.ExecuteScalarAsync();
            if (o == null || o == DBNull.Value) return null;
            return Convert.ToInt32(o);
        }

        // ===================== Compat: asegurar cliente para un usuario =====================
        public async Task<int> AsegurarClienteParaUsuarioAsync(int idUsuario)
        {
            await using var cn = new MySqlConnection(_connString);
            await cn.OpenAsync();

            // ¿existe cliente vinculado?
            int? idCliente = null;
            await using (var cmdSel = new MySqlCommand(
                "SELECT Id FROM clientes WHERE IdUsuario=@u LIMIT 1;", cn))
            {
                cmdSel.Parameters.AddWithValue("@u", idUsuario);
                var o = await cmdSel.ExecuteScalarAsync();
                if (o != null && o != DBNull.Value) idCliente = Convert.ToInt32(o);
            }

            if (idCliente.HasValue) return idCliente.Value;

            // lo creo vacío y devuelvo Id
            await using var cmdIns = new MySqlCommand(
                @"INSERT INTO clientes (IdUsuario, Dni, Direccion, Ciudad)
                  VALUES (@u, NULL, NULL, NULL);
                  SELECT LAST_INSERT_ID();", cn);
            cmdIns.Parameters.AddWithValue("@u", idUsuario);
            var nuevo = await cmdIns.ExecuteScalarAsync();
            return Convert.ToInt32(nuevo);
        }

        // ===================== NUEVO: Datos para PDF (cabecera + líneas) =====================
        /// <summary>
        /// Devuelve la cabecera y las líneas (alojamiento + servicios) para el PDF de la reserva.
        /// Devuelve false si la reserva no existe.
        /// </summary>
        public bool ObtenerReservaParaPdfConServicios(int idReserva, out PdfCabecera cab, out List<PdfLinea> lineas)
        {
            cab = default!;
            lineas = new List<PdfLinea>();

            using var cn = new MySqlConnection(_connString);
            cn.Open();

            // Cabecera + precio base para calcular noches
            const string sqlCab = @"
SELECT  r.Id,
        CONCAT(u.Nombre, ' ', IFNULL(u.Apellido,'')) AS Cliente,
        CONCAT(th.Nombre, ' Nº ', h.Numero)          AS Habitacion,
        r.FechaDesde,
        r.FechaHasta,
        r.Total,
        h.PrecioBase
FROM reservas r
JOIN usuarios u        ON u.Id  = r.IdUsuario
JOIN habitaciones h    ON h.Id  = r.IdHabitacion
JOIN tipo_habitacion th ON th.Id = h.TipoId
WHERE r.Id = @id;
";
            using var cmdCab = new MySqlCommand(sqlCab, cn);
            cmdCab.Parameters.AddWithValue("@id", idReserva);

            using var rd = cmdCab.ExecuteReader();
            if (!rd.Read())
            {
                // No existe la reserva
                return false;
            }

            var id = rd.GetInt32("Id");
            var cliente = rd.GetString("Cliente");
            var habitacion = rd.GetString("Habitacion");
            var fDesde = rd.GetDateTime("FechaDesde");
            var fHasta = rd.GetDateTime("FechaHasta");
            var total = rd.GetDecimal("Total");
            var precioBase = rd.GetDecimal("PrecioBase");
            rd.Close();

            cab = new PdfCabecera(id, cliente, habitacion, fDesde, fHasta, total);

            // Línea de alojamiento
            var noches = (fHasta.Date - fDesde.Date).Days;
            if (noches < 1) noches = 1;
            var subAloj = precioBase * noches;
            lineas.Add(new PdfLinea($"Alojamiento {habitacion} ({noches} noche/s)", precioBase, noches, subAloj));

            // Servicios adicionales (si los hay)
            const string sqlServ = @"
SELECT s.Nombre, rs.Precio, rs.Cantidad, (rs.Precio * rs.Cantidad) AS Subtotal
FROM reserva_servicio rs
JOIN servicios_adicionales s ON s.Id = rs.ServicioId
WHERE rs.ReservaId = @id;";
            using var cmdServ = new MySqlCommand(sqlServ, cn);
            cmdServ.Parameters.AddWithValue("@id", idReserva);

            using var rd2 = cmdServ.ExecuteReader();
            while (rd2.Read())
            {
                var concepto = rd2.GetString("Nombre");
                var precio = rd2.GetDecimal("Precio");
                var cant = rd2.GetInt32("Cantidad");
                var subt = rd2.GetDecimal("Subtotal");
                lineas.Add(new PdfLinea(concepto, precio, cant, subt));
            }
            rd2.Close();

            return true;
        }


        // ==== DTO de reporte mensual ====
        public record ReporteMensualDto(DateTime Dia, int Reservas, decimal Importe);

        // ==== Reporte mensual (por día) ====
        public List<ReporteMensualDto> ObtenerReporteMensual(int anio, int mes)
        {
            using var cn = new MySql.Data.MySqlClient.MySqlConnection(_connString);
            cn.Open();

            using var cmd = new MySql.Data.MySqlClient.MySqlCommand(@"
        SELECT DATE(r.FechaDesde)           AS Dia,
               COUNT(*)                     AS Reservas,
               COALESCE(SUM(r.Total), 0)    AS Importe
        FROM reservas r
        WHERE YEAR(r.FechaDesde) = @y
          AND MONTH(r.FechaDesde) = @m
        GROUP BY DATE(r.FechaDesde)
        ORDER BY Dia;", cn);

            cmd.Parameters.AddWithValue("@y", anio);
            cmd.Parameters.AddWithValue("@m", mes);

            using var rd = cmd.ExecuteReader();
            var list = new List<ReporteMensualDto>();

            while (rd.Read())
            {
                var dia = rd.GetDateTime("Dia");
                var reservas = Convert.ToInt32(rd["Reservas"]);
                var importe = rd.IsDBNull(rd.GetOrdinal("Importe"))
                                 ? 0m
                                 : Convert.ToDecimal(rd["Importe"]);
                list.Add(new ReporteMensualDto(dia, reservas, importe));
            }

            return list;
        }

    }
}
