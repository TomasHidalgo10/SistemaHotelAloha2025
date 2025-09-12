using SistemaHotelAloha.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.AccesoDatos
{
    public class TipoHabitacionInMemory
    {
        public static List<TipoHabitacion> TiposHabitacion { get; }

        static TipoHabitacionInMemory()
        {
            TiposHabitacion = new List<TipoHabitacion>
            {
                new TipoHabitacion(1,"Simple","Habitación individual con cama simple y baño privado."),
                new TipoHabitacion(2,"Doble","Habitación con cama matrimonial o dos camas individuales."),
                new TipoHabitacion(3,"Suite","Habitación amplia con sala de estar, vista panorámica y jacuzzi."),
                new TipoHabitacion(4,"Familiar","Habitación con capacidad para cuatro personas, ideal para familias.")
            };
        }
    }
}
