
using System.Collections.Generic;
using SistemaHotelAloha.Dominio;

namespace SistemaHotelAloha.Servicios
{
    public static class ClienteInMemory
    {
        public static List<Cliente> Clientes { get; } = new List<Cliente>();
    }
}
