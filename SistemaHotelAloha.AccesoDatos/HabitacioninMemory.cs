using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SistemaHotelAloha.Dominio;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaHotelAloha.AccesoDatos
{
    public static class HabitacionInMemory
    {
        public static List<Habitacion> Habitaciones { get; }

        static HabitacionInMemory()
        {
            var simple = new TipoHabitacion(1, "Simple", "Habitación individual con cama simple y baño privado.");
            var doble = new TipoHabitacion(2, "Doble", "Habitación doble con cama matrimonial o dos camas individuales.");
            var suite = new TipoHabitacion(3, "Suite", "Habitación amplia con sala de estar, vista panorámica y jacuzzi.");

            Habitaciones = new List<Habitacion>
            {
                new Habitacion("H001",101,simple,"Disponible",50.0f,"1 persona","Vista al jardín","Wifi"),
                new Habitacion("H002",102,doble,"Ocupada",80.0f,"2 personas","Vista al mar","Wifi, TV"),
                new Habitacion("H003",201,suite,"Mantenimiento",150.0f,"2 personas","Jacuzzi y balcón privado","Wifi, TV, Minibar")
            };
        }
    }
}
