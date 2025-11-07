using MySql.Data.MySqlClient;

namespace SistemaHotelAloha.AccesoDatos.Infra
{
    public static class MySqlConnectionFactory
    {
        public static MySqlConnection Create()
        {
            // Para MySql.Data (Oracle)
            var connectionString =
                "server=localhost;port=3306;database=aloha_db;uid=root;pwd=root;AllowPublicKeyRetrieval=true";

            return new MySqlConnection(connectionString);
        }
    }
}