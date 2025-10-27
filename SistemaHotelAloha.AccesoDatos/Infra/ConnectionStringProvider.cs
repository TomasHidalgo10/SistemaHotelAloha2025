using System;
using Microsoft.Extensions.Configuration;

namespace SistemaHotelAloha.AccesoDatos.Infra
{
    /// <summary>
    /// Lee la cadena "DefaultConnection" desde appsettings.json (copiado en la carpeta de salida)
    /// y la cachea en memoria. Si no existe, usa la variable de entorno ALOHA_CONNECTION
    /// y, como último recurso, una cadena por defecto a localhost.
    /// </summary>
    public static class ConnectionStringProvider
    {
        private static string? _cached;

        public static string Get()
        {
            if (!string.IsNullOrWhiteSpace(_cached))
                return _cached;

            // En Desktop, appsettings.json ya se copia a bin/ (PreserveNewest),
            // por lo que AppContext.BaseDirectory es la base correcta.
            var basePath = AppContext.BaseDirectory;

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            var config = builder.Build();

            var cs = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(cs))
            {
                // fallback a variable de entorno
                cs = Environment.GetEnvironmentVariable("ALOHA_CONNECTION");
            }

            if (string.IsNullOrWhiteSpace(cs))
            {
                // último fallback (útil en desarrollo)
                cs = "Server=127.0.0.1;Database=ALOHA_DB;Uid=root;Pwd=root;Port=3306";
            }

            _cached = cs;
            return cs;
        }

        public static string GetDefault() => Get();
    }
}