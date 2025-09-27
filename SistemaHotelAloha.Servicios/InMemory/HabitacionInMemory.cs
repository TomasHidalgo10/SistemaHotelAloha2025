
using System.Collections.Generic;
using SistemaHotelAloha.Dominio;

namespace SistemaHotelAloha.Servicios
{
    public static class HabitacionInMemory
    {
        public static List<Habitacion> Habitaciones { get; } = new List<Habitacion>();
    }
}
