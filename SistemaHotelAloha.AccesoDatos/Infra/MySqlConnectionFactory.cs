using MySql.Data.MySqlClient;

namespace SistemaHotelAloha.AccesoDatos.Infra
{
    public static class MySqlConnectionFactory
    {
        public static MySqlConnection Create()
        {
           
            var connectionString = ConnectionStringProvider.Get();
            return new MySqlConnection(connectionString);
        }
    }
}
