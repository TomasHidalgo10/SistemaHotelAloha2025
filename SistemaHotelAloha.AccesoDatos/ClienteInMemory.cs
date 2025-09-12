using SistemaHotelAloha.Dominio;

namespace SistemaHotelAloha.AccesoDatos
{
    public static class ClienteInMemory
    {
        public static List<Cliente> Clientes { get; }

        static ClienteInMemory()
        {
            Clientes = new List<Cliente>
            {
                new Cliente(1, "Juan", "Pérez","juan.perez@mail.com","1234","1111-2222","12345678",new DateTime(1990, 5, 12),"Argentina",DateTime.Now.AddMonths(-6)),
                new Cliente(2,"María","Gómez","maria.gomez@mail.com","abcd","3333-4444","87654321",new DateTime(1985, 8, 25),"Chile",DateTime.Now.AddMonths(-3)),
                new Cliente( 3, "Carlos", "López", "carlos.lopez@mail.com", "pass123", "5555-6666", "45678912", new DateTime(1995, 2, 14), "Uruguay", DateTime.Now.AddMonths(-7))
            };


        }
    }
}