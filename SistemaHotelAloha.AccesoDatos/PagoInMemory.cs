using SistemaHotelAloha.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.AccesoDatos
{
    public class PagoInMemory
    {
        public static List<Pago> Pagos { get; }

        static PagoInMemory()
        {
            Pagos = new List<Pago>
            {
                new Pago(1,250.0f,new DateTime(2025, 8, 1),"Tarjeta de Crédito","Pagado"),
                new Pago(2,150.5f,new DateTime(2025, 8, 5),"Efectivo","Pendiente"),
                new Pago(3,320.75f,new DateTime(2025, 8, 7),"Transferencia Bancaria","Pagado"),
                new Pago(4,80.0f,new DateTime(2025, 8, 9),"Tarjeta de Débito","Rechazado")
            };
        }
    }
}
