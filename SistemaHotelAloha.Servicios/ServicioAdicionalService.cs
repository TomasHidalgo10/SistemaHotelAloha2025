using SistemaHotelAloha.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SistemaHotelAloha.Servicios
{
    public class ServicioAdicionalService
    {
        private List<ServicioAdicional> serviciosAdicionales = new List<ServicioAdicional>();

        public void Add(ServicioAdicional servicio)
        {
            serviciosAdicionales.Add(servicio);
        }

        public List<ServicioAdicional> GetAll()
        {
            return serviciosAdicionales;
        }

        public ServicioAdicional? GetById(int id)
        {
            return serviciosAdicionales.FirstOrDefault(s => s.Id == id);
        }

        public bool Update(int id, string nombre, string descripcion, float precio)
        {
            var servicio = GetById(id);
            if (servicio == null)
                return false;
            servicio.Nombre = nombre;
            servicio.Descripcion = descripcion;
            servicio.Precio = precio;

            return true;
        }

        public bool Delete (int id)
        {
            var servicio = GetById(id);
            if (servicio == null)
                return false;
            serviciosAdicionales.Remove(servicio);
            return true;
        }

        internal ServicioAdicional GetOne(int servicioAdicionalId)
        {
            return serviciosAdicionales.FirstOrDefault(s => s.Id == servicioAdicionalId);
        }
    }
}
