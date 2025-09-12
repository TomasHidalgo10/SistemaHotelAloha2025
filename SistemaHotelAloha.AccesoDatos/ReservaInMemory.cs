using SistemaHotelAloha.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.AccesoDatos
{
    public class ReservaInMemory
    {
        public static List<Reserva> Reservas { get; }
        public static object Reserva { get; set; }

        static ReservaInMemory()
        {
            var simple = new TipoHabitacion(1, "Simple", "Habitación individual con cama simple y baño privado.");
            var doble = new TipoHabitacion(2, "Doble", "Habitación doble con cama matrimonial o dos camas individuales.");
            // Ejemplos de habitaciones
            var habitacion1 = new Habitacion("H001", 101, simple, "Disponible", 50.0f, "1 persona", "Vista al jardín", "Wifi"); 
            var habitacion2 = new Habitacion("H002", 102, doble, "Ocupada", 80.0f, "2 personas", "Vista al mar", "Wifi, TV"); 

            // Ejemplos de servicios adicionales
            var desayuno = new ServicioAdicional(123, "Desayuno Buffet", 50.0f, "Acceso al buffet libre del hotel con bebidas y comida ilimitada.");
            var spa = new ServicioAdicional(234, "Spa y Wellness", 100.5f, "Incluye sauna, masajes y acceso a piscina climatizada.");

            // Lista de reservas
            Reservas = new List<Reserva>
            {
                new Reserva(
                    1,
                    DateTime.Now.AddDays(-10),
                    DateTime.Now.AddDays(-5),
                    DateTime.Now.AddDays(-3),
                    "Finalizada",
                    140.0f,
                    "Tarjeta de Crédito",
                    habitacion1,
                    new List<ReservaServicio>
                    {
                        new ReservaServicio { cantidad = 2, subtotal = 20.0f, Servicio = desayuno },
                        new ReservaServicio { cantidad = 1, subtotal = 25.5f, Servicio = spa }
                    }
                ),
                new Reserva(
                    2,
                    DateTime.Now,
                    DateTime.Now.AddDays(3),
                    DateTime.Now.AddDays(5),
                    "Pendiente",
                    160.0f,
                    "Efectivo",
                    habitacion2,
                    new List<ReservaServicio>
                    {
                        new ReservaServicio { cantidad = 1, subtotal = 25.5f, Servicio = spa }
                    }
                )
            };
        }
    }
}
