using System;
using System.Data;
using MySql.Data.MySqlClient;
using SistemaHotelAloha.AccesoDatos.Infra;

namespace SistemaHotelAloha.AccesoDatos
{
    public class ServicioAdicionalAdoRepository
    {
        public DataTable GetAll()
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand("SELECT Id, Nombre, Precio, Descripcion FROM ServiciosAdicionales;", cn);
            using var da = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int Create(string nombre, float precio, string descripcion)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
                INSERT INTO ServiciosAdicionales (Nombre, Precio, Descripcion)
                VALUES (@Nombre, @Precio, @Descripcion);
                SELECT LAST_INSERT_ID();", cn);

            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@Precio", precio);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)descripcion ?? DBNull.Value);

            cn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(int id, string nombre, float precio, string descripcion)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
                UPDATE ServiciosAdicionales
                SET Nombre=@Nombre, Precio=@Precio, Descripcion=@Descripcion
                WHERE Id=@Id;", cn);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@Precio", precio);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)descripcion ?? DBNull.Value);

            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand("DELETE FROM ServiciosAdicionales WHERE Id=@Id;", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
