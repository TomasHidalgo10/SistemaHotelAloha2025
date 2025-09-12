using SistemaHotelAloha.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SistemaHotelAloha.Servicios
{
    public class RecepcionistaService
    {
        private List<Recepcionista> recepcionistas = new List<Recepcionista>();

        
        public void Add(Recepcionista recepcionista)
        {
            recepcionistas.Add(recepcionista);
        }

      
        public List<Recepcionista> GetAll()
        {
            return recepcionistas;
        }

       
        public Recepcionista? GetById(int id)
        {
            return recepcionistas.FirstOrDefault(r => r.Id == id);
        }

        
        public bool Update(int id, string nombre, string apellido, string email, string contraseña,
                           string telefono, string legajo, DateTime fechaContratacion, string turno)
        {
            var recepcionista = GetById(id);
            if (recepcionista == null)
                return false;

        
            recepcionista.SetNombre(nombre);
            recepcionista.SetApellido(apellido);
            recepcionista.SetEmail(email);
            recepcionista.SetContraseña(contraseña);
            recepcionista.SetTelefono(telefono);

            
            recepcionista.SetLegajo(legajo);
            recepcionista.SetFechaContratacion(fechaContratacion);
            recepcionista.SetTurno(turno);

            return true;
        }

        
        public bool Delete(int id)
        {
            var recepcionista = GetById(id);
            if (recepcionista == null)
                return false;

            recepcionistas.Remove(recepcionista);
            return true;
        }
    }
}
