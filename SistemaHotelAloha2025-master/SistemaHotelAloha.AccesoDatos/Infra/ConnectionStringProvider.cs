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
            // Si ya se leyó previamente, devolver desde memoria
            if (!string.IsNullOrWhiteSpace(_cached))
                return _cached!;

            // 1) Base path: carpeta del ejecutable (Desktop / Web)
            var basePath = AppContext.BaseDirectory;

            // 2) Intentar leer appsettings.json (como hacía Nico)
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            var config = builder.Build();
            var cs = config.GetConnectionString("DefaultConnection");

            // 3) Si no hay cadena en JSON, probar con variable de entorno
            if (string.IsNullOrWhiteSpace(cs))
                cs = Environment.GetEnvironmentVariable("ALOHA_CONNECTION");

            // 4) Fallback final (idéntico al de Nico, respetando formato)
            if (string.IsNullOrWhiteSpace(cs))
                cs = "Server=127.0.0.1;Database=ALOHA_DB;Uid=root;Pwd=root;Port=3306";

            // 5) Cachear y devolver
            _cached = cs!;
            return _cached!;
        }

        // Alias (para compatibilidad)
        public static string GetDefault() => Get();
    }
}