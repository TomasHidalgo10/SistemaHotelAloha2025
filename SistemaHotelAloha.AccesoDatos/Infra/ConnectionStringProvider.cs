using Microsoft.Extensions.Configuration;

namespace SistemaHotelAloha.AccesoDatos.Infra
{
    internal static class ConnectionStringProvider
    {
        private static IConfigurationRoot? _config;

        private static IConfigurationRoot GetConfig()
        {
            if (_config != null) return _config;

            var basePath = AppContext.BaseDirectory;
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var parent = Directory.GetParent(basePath)?.FullName;
            if (parent != null)
                builder.AddJsonFile(Path.Combine(parent, "appsettings.json"), optional: true, reloadOnChange: true);

            _config = builder.Build();
            return _config;
        }

        public static string GetDefault()
        {
            var cfg = GetConfig();
            var cs = cfg.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("No se encontró 'ConnectionStrings:DefaultConnection' en appsettings.json.");
            return cs!;
        }
    }
}