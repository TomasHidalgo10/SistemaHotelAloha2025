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
        public DataTable GetAll()
        {
            using var cn = MySqlConnectionFactory.Create();
            using var da = new MySqlDataAdapter(@"
                SELECT Id, Numero, TipoHabitacion, Estado, PrecioBase
                FROM habitaciones
                ORDER BY Id DESC;", cn);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // Devuelve nuevo Id o 0 si falló
        public int Create(int numero, int tipoHabitacion, string estado, decimal precioBase)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
                INSERT INTO habitaciones (Numero, TipoHabitacion, Estado, PrecioBase)
                VALUES (@Numero, @Tipo, @Estado, @Precio);
                SELECT LAST_INSERT_ID();", cn);
            cmd.Parameters.AddWithValue("@Numero", numero);
            cmd.Parameters.AddWithValue("@Tipo", tipoHabitacion);
            cmd.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Disponible" : estado);
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

        // Devuelve filas afectadas
        public int Update(int id, int numero, int tipoHabitacion, string estado, decimal precioBase)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
                UPDATE habitaciones
                SET Numero=@Numero, TipoHabitacion=@Tipo, Estado=@Estado, PrecioBase=@Precio
                WHERE Id=@Id;", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Numero", numero);
            cmd.Parameters.AddWithValue("@Tipo", tipoHabitacion);
            cmd.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Disponible" : estado);
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

        // Devuelve filas afectadas
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
    }
}