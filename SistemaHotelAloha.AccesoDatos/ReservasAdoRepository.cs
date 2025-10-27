
using MySql.Data.MySqlClient;
using SistemaHotelAloha.AccesoDatos.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SistemaHotelAloha.AccesoDatos
{
    public class ReservasAdoRepository
    {
        public sealed record ReservaDto(int Id, int HabitacionId, string HabitacionLabel, DateTime Desde, DateTime Hasta, decimal PrecioTotal, string Estado);

        public int Crear(int usuarioId, int habitacionId, DateTime desde, DateTime hasta, decimal precioTotal)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();

            // Validación de solapamiento en la misma habitación
            using (var chk = new MySqlCommand(@"
SELECT COUNT(1) FROM Reservas
WHERE HabitacionId=@H AND Estado<>'Cancelada'
  AND NOT (@Hasta <= FechaDesde OR @Desde >= FechaHasta);", cn))
            {
                chk.Parameters.AddWithValue("@H", habitacionId);
                chk.Parameters.AddWithValue("@Desde", desde);
                chk.Parameters.AddWithValue("@Hasta", hasta);
                if (Convert.ToInt32(chk.ExecuteScalar()) > 0) return -100; // ya ocupada
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

            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public List<ReservaDto> GetByUsuario(int usuarioId)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
SELECT r.Id, r.HabitacionId, CONCAT('Hab ', h.Numero, ' (', COALESCE(h.TipoHabitacion,'Standard'), ')') as HabLabel,
       r.FechaDesde, r.FechaHasta, r.PrecioTotal, r.Estado
FROM Reservas r
JOIN Habitaciones h ON h.Id = r.HabitacionId
WHERE r.UsuarioId=@U
ORDER BY r.Id DESC;", cn);

            cmd.Parameters.AddWithValue("@U", usuarioId);

            cn.Open();
            using var rd = cmd.ExecuteReader();
            var list = new List<ReservaDto>();
            while (rd.Read())
            {
                var id = rd.GetInt32("Id");
                var hid = rd.GetInt32("HabitacionId");
                var label = rd["HabLabel"]?.ToString() ?? $"Hab {hid}";
                var desde = Convert.ToDateTime(rd["FechaDesde"]);
                var hasta = Convert.ToDateTime(rd["FechaHasta"]);
                var precio = Convert.ToDecimal(rd["PrecioTotal"]);
                var estado = rd["Estado"]?.ToString() ?? "Confirmada";
                list.Add(new ReservaDto(id, hid, label, desde, hasta, precio, estado));
            }
            return list;
        }

        public bool Cancelar(int id, int usuarioId)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
UPDATE Reservas SET Estado='Cancelada'
WHERE Id=@Id AND UsuarioId=@U;", cn);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@U", usuarioId);

            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}