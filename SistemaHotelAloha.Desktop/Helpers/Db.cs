using Microsoft.Extensions.Configuration;   // <— IMPORTANTE
using MySql.Data.MySqlClient;
using System;

namespace SistemaHotelAloha.Desktop.Helpers
{
    public static class Db
    {
        private static readonly string _connStr;

        static Db()
        {
            var cfg = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            _connStr = cfg.GetConnectionString("DefaultConnection") ?? "";
            if (string.IsNullOrWhiteSpace(_connStr))
                throw new InvalidOperationException("ConnectionStrings:DefaultConnection no configurada.");
        }

        public static MySqlConnection Conn() => new MySqlConnection(_connStr);
    }
}