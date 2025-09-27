using System;
using System.Data;
using MySql.Data.MySqlClient;
using SistemaHotelAloha.AccesoDatos.Infra;

namespace SistemaHotelAloha.AccesoDatos
{
    public class HabitacionAdoRepository
    {
        public DataTable GetAll()
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand("SELECT Id, Numero, TipoHabitacion, Estado, PrecioBase, Capacidad, Descripcion, Servicio FROM Habitaciones;", cn);
            using var da = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public bool Create(string id, int numero, int tipoHabitacion, string estado, float precioBase, string capacidad, string descripcion, string servicio)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
                INSERT INTO Habitaciones (Id, Numero, TipoHabitacion, Estado, PrecioBase, Capacidad, Descripcion, Servicio)
                VALUES (@Id, @Numero, @TipoHabitacion, @Estado, @PrecioBase, @Capacidad, @Descripcion, @Servicio);", cn);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Numero", numero);
            cmd.Parameters.AddWithValue("@TipoHabitacion", tipoHabitacion);
            cmd.Parameters.AddWithValue("@Estado", estado);
            cmd.Parameters.AddWithValue("@PrecioBase", precioBase);
            cmd.Parameters.AddWithValue("@Capacidad", (object?)capacidad ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Descripcion", descripcion);
            cmd.Parameters.AddWithValue("@Servicio", servicio);

            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Update(string id, int numero, int tipoHabitacion, string estado, float precioBase, string capacidad, string descripcion, string servicio)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
                UPDATE Habitaciones
                SET Numero=@Numero, TipoHabitacion=@TipoHabitacion, Estado=@Estado, PrecioBase=@PrecioBase, Capacidad=@Capacidad, Descripcion=@Descripcion, Servicio=@Servicio
                WHERE Id=@Id;", cn);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Numero", numero);
            cmd.Parameters.AddWithValue("@TipoHabitacion", tipoHabitacion);
            cmd.Parameters.AddWithValue("@Estado", estado);
            cmd.Parameters.AddWithValue("@PrecioBase", precioBase);
            cmd.Parameters.AddWithValue("@Capacidad", (object?)capacidad ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Descripcion", descripcion);
            cmd.Parameters.AddWithValue("@Servicio", servicio);

            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(string id)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand("DELETE FROM Habitaciones WHERE Id=@Id;", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
