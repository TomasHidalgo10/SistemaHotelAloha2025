using MySql.Data.MySqlClient;
using SistemaHotelAloha.AccesoDatos.Infra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.AccesoDatos
{
    public class EstadoReservaLookupAdo
    {
        public DataTable GetAll()
        {
            using var cn = MySqlConnectionFactory.Create();
            using var da = new MySqlDataAdapter(
                "SELECT Id, Nombre FROM estado_reserva ORDER BY Nombre;", cn);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}