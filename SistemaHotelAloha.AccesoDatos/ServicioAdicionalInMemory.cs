using SistemaHotelAloha.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.AccesoDatos
{
    public class ServicioAdicionalInMemory
    {
        public static List<ServicioAdicional> ServiciosAdicionales { get; }

        static ServicioAdicionalInMemory()
        {
            ServiciosAdicionales = new List<ServicioAdicional>
            {
                new ServicioAdicional(123,"Desayuno Buffet",50.0f,"Acceso al buffet libre del hotel con bebidas y comida ilimitada."),
                new ServicioAdicional(234,"Spa y Wellness",100.5f,"Incluye sauna, masajes y acceso a piscina climatizada."),
                new ServicioAdicional(345,"Estacionamiento Cubierto",80.0f,"Cochera techada con vigilancia 24 horas."),
                new ServicioAdicional(456,"Traslado al Aeropuerto",15.0f,"Servicio de transporte privado desde/hacia el aeropuerto.")
            };
        }
    }
}
