using MySql.Data.MySqlClient;

namespace SistemaHotelAloha.AccesoDatos.Infra
{
    public static class MySqlConnectionFactory
    {
        public static MySqlConnection Create()
        {
            var cs = ConnectionStringProvider.GetDefault();
            return new MySqlConnection(cs);
        }
    }
}