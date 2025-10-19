using SistemaHotelAloha.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.Servicios
{
    public class ReservaServicioService
    {
        private readonly ReservaService _reservaService;
        private readonly ServicioAdicionalService _servicioAdicionalService;

        public ReservaServicioService()
        {
            _reservaService = new ReservaService();
            _servicioAdicionalService = new ServicioAdicionalService();
        }

        public ReservaServicio AgregarServicioAReserva(int reservaId, int servicioAdicionalId, int cantidad)
        {
            // 1. Encontra la reserva y el servicio adicional por sus IDs.
            var reserva = _reservaService.GetOne(reservaId);
            if (reserva == null)
            {
                throw new KeyNotFoundException($"No se encontró la reserva con ID {reservaId}.");
            }

            var servicioAdicional = _servicioAdicionalService.GetOne(servicioAdicionalId);
            if (servicioAdicional == null)
            {
                throw new KeyNotFoundException($"No se encontró el servicio adicional con ID {servicioAdicionalId}.");
            }

            // 2. Crea una nueva instancia de ReservaServicio
            var nuevoReservaServicio = new ReservaServicio();
            nuevoReservaServicio.SetCantidad(cantidad);
            nuevoReservaServicio.SetServicio(servicioAdicional);

            // 3. Calcula el subtotal y aplicarlo.
            float subtotalCalculado = cantidad * servicioAdicional.Precio;
            nuevoReservaServicio.SetSubtotal(subtotalCalculado);

            // 4. Agrega la instancia de ReservaServicio a la lista de servicios de la reserva.
            reserva.AgregarServicio(nuevoReservaServicio);

            Console.WriteLine($"Se agregó el servicio '{servicioAdicional.Nombre}' a la reserva con ID {reserva.Id}.");
            return nuevoReservaServicio;
        }

        public bool EliminarServicioDeReserva(int reservaId, int servicioAdicionalId)
        {
            var reserva = _reservaService.GetOne(reservaId);
            if (reserva == null)
            {
                return false;
            }

            var reservaServicioToRemove = reserva.Servicios
                .FirstOrDefault(rs => rs.Servicio.Id == servicioAdicionalId);

            if (reservaServicioToRemove != null)
            {
                reserva.EliminarServicio(reservaServicioToRemove);
                return true;
            }
            return false;
        }
    }
}
