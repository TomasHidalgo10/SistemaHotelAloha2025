using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaHotelAloha.AccesoDatos;
using SistemaHotelAloha.Dominio;

namespace SistemaHotelAloha.Servicios
{
    public class HabitacionService
    {
        public void Add(Habitacion habitacion) 
        {
            habitacion.Id ??=Guid.NewGuid().ToString();
            HabitacionInMemory.Habitaciones.Add(habitacion);
        }

        public List<Habitacion> GetAll() 
        {
            return HabitacionInMemory.Habitaciones.ToList();
        }

        public Habitacion GetById(string id) 
        {
            return HabitacionInMemory.Habitaciones.Find(h => h.Id == id)
                ?? throw new KeyNotFoundException($"Habitación con ID {id} no encontrado.");
        }

        public bool Delete(string id) 
        {
            var habitacion = HabitacionInMemory.Habitaciones.Find(h => h.Id == id);
            if (habitacion != null) 
            {
                HabitacionInMemory.Habitaciones.Remove(habitacion);
                return true;
            }
            return false;
        }

        public void Uptade(Habitacion habitacionActualizada) 
        {
            var index = HabitacionInMemory.Habitaciones.FindIndex(h => h.Id == habitacionActualizada.Id);
            if (index == -1)
                throw new KeyNotFoundException($"Habitación con ID {habitacionActualizada.Id} no encontrada.");

            HabitacionInMemory.Habitaciones[index] = habitacionActualizada;

        }

    }

}
