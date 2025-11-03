using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SistemaHotelAloha.AccesoDatos.Infra
{
    /// <summary>
    /// Lee "ConnectionStrings:DefaultConnection" desde appsettings.json (copiado al output),
    /// si no existe, usa la variable de entorno ALOHA_CONNECTION,
    /// y como último recurso, una cadena local por defecto.
    /// </summary>
    public static class ConnectionStringProvider
    {
        private static string? _cached;

        public static string Get()
        {
            if (!string.IsNullOrWhiteSpace(_cached))
                return _cached!;

            // 1) Variable de entorno (tiene prioridad si está definida)
            var env = Environment.GetEnvironmentVariable("ALOHA_CONNECTION");
            if (!string.IsNullOrWhiteSpace(env))
            {
                _cached = env!;
                return _cached!;
            }

            // 2) appsettings.json (en la carpeta del ejecutable)
            try
            {
                var basePath = AppContext.BaseDirectory;
                var jsonPath = Path.Combine(basePath, "appsettings.json");

                if (File.Exists(jsonPath))
                {
                    var cfg = new ConfigurationBuilder()
                        .SetBasePath(basePath)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .Build();

                    var cs = cfg.GetConnectionString("DefaultConnection");
                    if (!string.IsNullOrWhiteSpace(cs))
                    {
                        _cached = cs!;
                        return _cached!;
                    }
                }
            }
            catch
            {
                // Ignorar y caer al fallback
            }

            // 3) Fallback (desarrollo)
            _cached = "Server=127.0.0.1;Port=3306;Database=aloha_db;Uid=root;Pwd=root;AllowPublicKeyRetrieval=true;SslMode=None";
            return _cached!;
        }

        public static string GetDefault() => Get();
    }
}
