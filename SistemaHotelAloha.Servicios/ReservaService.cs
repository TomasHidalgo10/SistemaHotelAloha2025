using SistemaHotelAloha.AccesoDatos;
using SistemaHotelAloha.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.Servicios
{
    public class ReservaService
    {
        private static List<Reserva> _reservas = new List<Reserva>();
        private static int _nextId = 1;


        public List<Reserva> GetAll()
        {
            return _reservas;
        }

        public Reserva? GetOne(int id)
        {
            return _reservas.FirstOrDefault(r => r.Id == id) ;
        }

        public Reserva Create(Reserva reserva)
        {
            if (reserva == null)
            {
                throw new ArgumentNullException(nameof(reserva));
            }

            reserva.SetId(_nextId++);
            // Se calcula el total antes de guardar la reserva.
            reserva.SetTotal((float)reserva.CalcularTotal());
            _reservas.Add(reserva);
            return reserva;
        }

        public Reserva? Update(Reserva reserva)
        {
            if (reserva == null)
            {
                throw new ArgumentNullException(nameof(reserva));
            }

            var existingReserva = _reservas.FirstOrDefault(r => r.Id == reserva.Id);
            if (existingReserva == null)
            {
                return null;
            }

            // Actualizar las propiedades de la reserva existente
            existingReserva.SetFechaCreacion(reserva.FechaCreacion);
            existingReserva.SetFechaCheckIn(reserva.FechaCheckIn);
            existingReserva.SetFechaCheckOut(reserva.FechaCheckOut);
            existingReserva.SetEstado(reserva.Estado);
            existingReserva.SetMetodoPago(reserva.MetodoPago);

            // Actualizar las relaciones
            existingReserva.Habitacion = reserva.Habitacion;
            existingReserva.Servicios = reserva.Servicios;

            // Recalcular el total después de las actualizaciones
            existingReserva.SetTotal((float)existingReserva.CalcularTotal());

            return existingReserva;
        }

        public bool Delete(int id)
        {
            var reservaToRemove = _reservas.FirstOrDefault(r => r.Id == id);
            if (reservaToRemove == null)
            {
                return false;
            }

            return _reservas.Remove(reservaToRemove);
        }
    }
}
