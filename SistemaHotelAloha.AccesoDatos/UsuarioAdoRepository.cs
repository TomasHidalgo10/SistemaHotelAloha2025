using System;
using System.Data;
using MySql.Data.MySqlClient;
using SistemaHotelAloha.AccesoDatos.Infra;

namespace SistemaHotelAloha.AccesoDatos
{
    // Repositorio ADO.NET desacoplado del Dominio: usa DataTable y parámetros simples
    public class UsuarioAdoRepository
    {
        public DataTable GetAll()
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand("SELECT Id, Nombre, Apellido, Email, Telefono, FechaRegistro, Activo FROM Usuarios;", cn);
            using var da = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable GetById(int id)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand("SELECT Id, Nombre, Apellido, Email, Telefono, FechaRegistro, Activo FROM Usuarios WHERE Id=@Id;", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var da = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int Create(string nombre, string apellido, string email, string contraseña, string telefono, DateTime fechaRegistro, bool activo)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
                INSERT INTO Usuarios (Nombre, Apellido, Email, Contraseña, Telefono, FechaRegistro, Activo)
                VALUES (@Nombre, @Apellido, @Email, @Contrasena, @Telefono, @FechaRegistro, @Activo);
                SELECT LAST_INSERT_ID();", cn);

            cmd.Parameters.AddWithValue("@Nombre", (object?)nombre ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Apellido", (object?)apellido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Contrasena", (object?)contraseña ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", (object?)telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaRegistro", fechaRegistro);
            cmd.Parameters.AddWithValue("@Activo", activo);

            cn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(int id, string nombre, string apellido, string email, string contraseña, string telefono, DateTime fechaRegistro, bool activo)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
                UPDATE Usuarios
                SET Nombre=@Nombre, Apellido=@Apellido, Email=@Email, Contraseña=@Contrasena, Telefono=@Telefono, FechaRegistro=@FechaRegistro, Activo=@Activo
                WHERE Id=@Id;", cn);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Nombre", (object?)nombre ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Apellido", (object?)apellido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Contrasena", (object?)contraseña ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", (object?)telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaRegistro", fechaRegistro);
            cmd.Parameters.AddWithValue("@Activo", activo);

            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand("DELETE FROM Usuarios WHERE Id=@Id;", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
