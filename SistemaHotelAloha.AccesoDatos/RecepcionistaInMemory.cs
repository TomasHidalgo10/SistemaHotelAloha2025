using SistemaHotelAloha.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.AccesoDatos
{
    public class RecepcionistaInMemory
    {
        public static List<Recepcionista> Recepcionistas { get; }

        static RecepcionistaInMemory()
        {
            Recepcionistas = new List<Recepcionista>
            {
                new Recepcionista(1,"Pedro","López","pedro.lopez@mail.com","pass123","5555-6666","R001",new DateTime(2020, 3, 15),"Mañana",DateTime.Now.AddYears(-3)),
                new Recepcionista(2,"Laura","Martínez","laura.martinez@mail.com","pass456","7777-8888","R002",new DateTime(2021, 7, 1),"Tarde",DateTime.Now.AddYears(-2)),
                new Recepcionista(3,"Andrés","Fernández","andres.fernandez@mail.com","pass789","9999-0000","R003",new DateTime(2022, 5, 10),"Noche",DateTime.Now.AddYears(-1))
            };
        }

    }
}
