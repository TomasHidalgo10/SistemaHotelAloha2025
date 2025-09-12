using SistemaHotelAloha.AccesoDatos;
using SistemaHotelAloha.Dominio;

namespace SistemaHotelAloha.Servicios
{
    public class ClienteService
    {
        public void Add(Cliente cliente) 
        {   ClienteInMemory.Clientes.Add(cliente);

        }

        public bool UpdateCliente(int id, string nombre, string apellido, string email, string contraseña,
                             string telefono, string dni, DateTime fechaNacimiento, string nacionalidad)
        {
            var cliente = GetById(id);
            if (cliente == null)
                return false; 

           
            cliente.SetNombre(nombre);
            cliente.SetApellido(apellido);
            cliente.SetEmail(email);
            cliente.SetContraseña(contraseña);
            cliente.SetTelefono(telefono);

            
            cliente.SetDni(dni);
            cliente.SetFechaNacimiento(fechaNacimiento);
            cliente.SetNacionalidad(nacionalidad);

            return true;
        }

        public bool Delete(int id) 
        {
            Cliente? clienteToDelete = ClienteInMemory.Clientes.Find(c => c.Id == id);
            if (clienteToDelete != null)
            {
                ClienteInMemory.Clientes.Remove(clienteToDelete);
                return true;
            }
            else
            {
                return false; //Nos conviene lanzar una excepcion?
            }
        }
        public Cliente GetById(int id) 
        { 
            return ClienteInMemory.Clientes.Find(c => c.Id == id) ?? throw new KeyNotFoundException($"Cliente con ID {id} no encontrado.");
        }
        public List<Cliente> GetAll() 
        { 
            return ClienteInMemory.Clientes.ToList();
        }

    }
}
