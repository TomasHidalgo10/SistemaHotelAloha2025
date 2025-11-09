using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaHotelAloha.AccesoDatos;
using SistemaHotelAloha.Dominio;

namespace SistemaHotelAloha.Servicios
{
    public class TipoHabitacionService
    {
        private List<TipoHabitacion> tiposHabitacion = new List<TipoHabitacion>();

        public void Add(TipoHabitacion tipo)
        {
            tiposHabitacion.Add(tipo);
        }

        public List<TipoHabitacion> GetAll()
        {
            return tiposHabitacion;
        }

        public TipoHabitacion? GetById(int id)
        {
            return tiposHabitacion.FirstOrDefault(t => t.Id == id);
        }

        public bool Update(int id, string nombre, string descripcion)
        {
            var tipo = GetById(id);
            if (tipo == null)
                return false;
            tipo.Nombre = nombre;
            tipo.Descripcion = descripcion;
            return true;
        }

        public bool Delete(int id)
        {
            var tipo = GetById(id);
            if (tipo == null)
                return false;
            tiposHabitacion.Remove(tipo);
            return true;
        }

    }
}
