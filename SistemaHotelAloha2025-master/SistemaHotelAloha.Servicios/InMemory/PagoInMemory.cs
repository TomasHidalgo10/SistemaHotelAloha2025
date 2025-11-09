
using System.Collections.Generic;
using SistemaHotelAloha.Dominio;

namespace SistemaHotelAloha.Servicios
{
    public static class PagoInMemory
    {
        public static List<Pago> Pagos { get; } = new List<Pago>();
    }
}
