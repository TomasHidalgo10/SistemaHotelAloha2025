using System;
using System.Data;
using SistemaHotelAloha.AccesoDatos.Infra;
using MySql.Data.MySqlClient;

namespace SistemaHotelAloha.AccesoDatos
{
    /// <summary>
    /// ADO.NET puro para la tabla Usuarios.
    /// Convenciones:
    /// - Create  => devuelve nuevo Id (>0). Si email duplicado => -1062. Otro error => 0
    /// - Update  => devuelve filas afectadas (>=1). Si email duplicado => -1062. Otro error => 0
    /// - Delete  => devuelve filas afectadas (>=1). Otro error => 0
    /// - GetAll  => DataTable
    /// </summary>
    public class UsuarioAdoRepository
    {
        // ---------- SELECTS ----------
        public DataTable GetAll()
        {
            using var cn = MySqlConnectionFactory.Create();
            using var da = new MySqlDataAdapter(
                @"SELECT Id, Nombre, Apellido, Email, Telefono, FechaRegistro, Activo
                  FROM Usuarios
                  ORDER BY Id DESC;", cn);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public bool EmailExists(string email)
        {
            using var cn = MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand("SELECT COUNT(1) FROM Usuarios WHERE Email=@Email;", cn);
            cmd.Parameters.AddWithValue("@Email", email);
            cn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public int Authenticate(string email, string contraseña)
        {
            using var cn = Infra.MySqlConnectionFactory.Create();
            using var cmd = new MySqlCommand(@"
        SELECT Id
        FROM Usuarios
        WHERE Email=@Email AND Contraseña=@Contraseña AND Activo=1
        LIMIT 1;", cn);

            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Contraseña", contraseña);

            cn.Open();
            var obj = cmd.ExecuteScalar();
            return obj == null ? 0 : Convert.ToInt32(obj);
        }

        public DataTable GetById(int id)
        {
            using var cn = Infra.MySqlConnectionFactory.Create();
            using var da = new MySqlDataAdapter(
                "SELECT Id, Nombre, Apellido, Email FROM Usuarios WHERE Id=@Id;",
                cn);
            da.SelectCommand.Parameters.AddWithValue("@Id", id);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        // ---------- CREATE ----------
        // Devuelve: nuevo Id (>0). Si email duplicado => -1062. Otro error => 0
        public int Create(string nombre, string apellido, string email, string contraseña, string telefono, DateTime fechaRegistro, bool activo)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();

            // 1) Duplicado de email (preventivo)
            using (var chk = new MySqlCommand("SELECT COUNT(1) FROM Usuarios WHERE Email=@Email;", cn))
            {
                chk.Parameters.AddWithValue("@Email", email);
                if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                    return -1062;
            }

            // 2) Normalizar contraseña si la columna es NOT NULL
            if (string.IsNullOrWhiteSpace(contraseña))
                contraseña = "changeme";

            // 3) Insert + devolver LAST_INSERT_ID()
            using var cmd = new MySqlCommand(@"
                INSERT INTO Usuarios (Nombre, Apellido, Email, Contraseña, Telefono, FechaRegistro, Activo)
                VALUES (@Nombre, @Apellido, @Email, @Contraseña, @Telefono, @FechaRegistro, @Activo);
                SELECT LAST_INSERT_ID();", cn);

            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@Apellido", (object?)apellido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Contraseña", contraseña);
            cmd.Parameters.AddWithValue("@Telefono", (object?)telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaRegistro", fechaRegistro);
            cmd.Parameters.AddWithValue("@Activo", activo);

            try
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                return -1062; // clave duplicada (Email único)
            }
            catch
            {
                return 0;
            }
        }

        // ---------- UPDATE ----------
        // Devuelve: filas afectadas (>=1). Si email duplicado => -1062. Otro error => 0
        public int Update(int id, string nombre, string apellido, string email, string contraseña, string telefono, DateTime fechaRegistro, bool activo)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();

            // 1) Duplicado de email en otro usuario
            using (var chk = new MySqlCommand("SELECT COUNT(1) FROM Usuarios WHERE Email=@Email AND Id<>@Id;", cn))
            {
                chk.Parameters.AddWithValue("@Email", email);
                chk.Parameters.AddWithValue("@Id", id);
                if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                    return -1062;
            }

            // 2) Normalización mínima
            if (string.IsNullOrWhiteSpace(contraseña))
                contraseña = "changeme";

            // 3) Update
            using var cmd = new MySqlCommand(@"
                UPDATE Usuarios
                SET Nombre=@Nombre, Apellido=@Apellido, Email=@Email, Contraseña=@Contraseña,
                    Telefono=@Telefono, FechaRegistro=@FechaRegistro, Activo=@Activo
                WHERE Id=@Id;", cn);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@Apellido", (object?)apellido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Contraseña", contraseña);
            cmd.Parameters.AddWithValue("@Telefono", (object?)telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaRegistro", fechaRegistro);
            cmd.Parameters.AddWithValue("@Activo", activo);

            try
            {
                return cmd.ExecuteNonQuery(); // filas afectadas
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                return -1062;
            }
            catch
            {
                return 0;
            }
        }
        public string GetNombreCompletoById(int id)
        {
            using var cn = Infra.MySqlConnectionFactory.Create();
            using var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                "SELECT CONCAT(Nombre, ' ', Apellido) FROM Usuarios WHERE Id=@Id LIMIT 1;", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            cn.Open();
            var obj = cmd.ExecuteScalar();
            return obj == null ? "" : obj.ToString()!;
        }

        // ---------- DELETE ----------
        // Devuelve: filas afectadas (>=1). Otro error => 0
        public int Delete(int id)
        {
            using var cn = MySqlConnectionFactory.Create();
            cn.Open();
            using var cmd = new MySqlCommand("DELETE FROM Usuarios WHERE Id=@Id;", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch
            {
                return 0;
            }
        }
        public bool Create(string nombre, string apellido, string email, string contraseñaHash, string telefono)
        {
            var id = Create(
                nombre: nombre,
                apellido: apellido,
                email: email,
                contraseña: contraseñaHash,
                telefono: telefono,
                fechaRegistro: DateTime.UtcNow,
                activo: true
            );
            return id > 0;
        }
    }
}
