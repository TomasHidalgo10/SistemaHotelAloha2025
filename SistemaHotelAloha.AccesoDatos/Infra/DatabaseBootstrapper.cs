using System;
using System.Text;
using MySqlConnector;

namespace SistemaHotelAloha.AccesoDatos.Infra
{
    public static class DatabaseBootstrapper
    {
        public static void EnsureCreated()
        {
            // 1) Leer la cadena completa (con Database)
            var fullConn = ConnectionStringProvider.Get(); // <- YA EXISTE EN TU SOLUCIÓN

            // 2) Construir conexiones: una SIN Database (para crearla) y otra CON Database
            var full = new MySqlConnectionStringBuilder(fullConn);
            var dbName = full.Database;

            // Conexión al servidor sin DB
            var serverBuilder = new MySqlConnectionStringBuilder(fullConn)
            {
                Database = "" // quitamos la BD para poder crearla
            };

            using (var cnServer = new MySqlConnection(serverBuilder.ToString()))
            {
                cnServer.Open();
                using var cmd = cnServer.CreateCommand();
                cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS `{dbName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
                cmd.ExecuteNonQuery();
            }

            // 3) Crear tablas (IF NOT EXISTS) dentro de la DB
            using (var cn = new MySqlConnection(fullConn))
            {
                cn.Open();
                using var cmd = cn.CreateCommand();
                cmd.CommandText = BuildDdl();
                cmd.ExecuteNonQuery();
            }
        }

        private static string BuildDdl()
        {
            // DDL alineado a tus repos ADO (tipos y nombres que usan tus Create/Update):
            var sb = new StringBuilder();

            sb.AppendLine(@"
CREATE TABLE IF NOT EXISTS Usuarios (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre        VARCHAR(100) NOT NULL,
    Apellido      VARCHAR(100) NULL,
    Email         VARCHAR(150) NOT NULL UNIQUE,
    Contraseña    VARCHAR(255) NOT NULL,
    Telefono      VARCHAR(50),
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
    Activo        BOOLEAN DEFAULT TRUE
);");

            sb.AppendLine(@"
CREATE TABLE IF NOT EXISTS Habitaciones (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Numero         INT NOT NULL,
    Capacidad      INT DEFAULT 2,
    TipoHabitacion INT DEFAULT 1,
    PrecioBase     FLOAT NOT NULL,
    Descripcion    VARCHAR(255),
    Estado         VARCHAR(50) DEFAULT 'Disponible',
    Servicio       VARCHAR(100)
);");

            sb.AppendLine(@"
CREATE TABLE IF NOT EXISTS ServiciosAdicionales (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre      VARCHAR(100) NOT NULL,
    Precio      FLOAT NOT NULL,
    Descripcion VARCHAR(255)
);");

            sb.AppendLine(@"
CREATE TABLE IF NOT EXISTS Reservas (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdUsuario    INT NOT NULL,
    IdHabitacion INT NOT NULL,
    FechaIngreso DATE NOT NULL,
    FechaEgreso  DATE NOT NULL,
    Total        FLOAT NOT NULL,
    Estado       VARCHAR(50) DEFAULT 'Pendiente',
    CONSTRAINT FK_Reservas_Usuarios    FOREIGN KEY (IdUsuario)    REFERENCES Usuarios(Id)    ON DELETE CASCADE,
    CONSTRAINT FK_Reservas_Habitaciones FOREIGN KEY (IdHabitacion) REFERENCES Habitaciones(Id) ON DELETE CASCADE
);");

            sb.AppendLine(@"
CREATE TABLE IF NOT EXISTS ReservaServicios (
    IdReserva INT NOT NULL,
    IdServicio INT NOT NULL,
    Cantidad INT DEFAULT 1,
    PRIMARY KEY (IdReserva, IdServicio),
    CONSTRAINT FK_ReservaServicios_Reservas  FOREIGN KEY (IdReserva)  REFERENCES Reservas(Id)            ON DELETE CASCADE,
    CONSTRAINT FK_ReservaServicios_Servicios FOREIGN KEY (IdServicio) REFERENCES ServiciosAdicionales(Id) ON DELETE CASCADE
);");

            return sb.ToString();
        }
    }
}