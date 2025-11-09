using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.Dominio
{
    public class Reserva
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaCheckIn { get; set; }
        public DateTime FechaCheckOut { get; set; }
        public string Estado { get; set; } = null!;
        public float Total { get; set; }
        public string MetodoPago { get; set; } = null!;

        public Habitacion Habitacion { get; set; } = null!;

        public List<ReservaServicio> Servicios { get; set; }
        public Reserva()
        {
            Servicios = new List<ReservaServicio>();
        }

        public Reserva(int id, DateTime fechaCreacion, DateTime fechaCheckIn, DateTime fechaCheckOut, string estado, float total, string metodoPago, Habitacion habitacion, List<ReservaServicio> servicios)
        {
            this.Id = id;
            this.FechaCreacion = fechaCreacion;
            this.FechaCheckIn = fechaCheckIn;
            this.FechaCheckOut = fechaCheckOut;
            this.Estado = estado;
            this.Total = total;
            this.MetodoPago = metodoPago;
            this.Habitacion = habitacion;
            this.Servicios = servicios ?? new List<ReservaServicio>();
        }


        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la reserva debe ser un número positivo.", nameof(id));
            this.Id = id;
        }

        public void SetFechaCreacion(DateTime fechaCreacion)
        {
            if (fechaCreacion > DateTime.Now)
                throw new ArgumentException("La fecha de creación no puede ser futura.", nameof(fechaCreacion));
            this.FechaCreacion = fechaCreacion;
        }

        public void SetFechaCheckIn(DateTime fechaCheckIn)
        {
            if (fechaCheckIn < DateTime.Now)
                throw new ArgumentException("La fecha de check-in no puede ser pasada.", nameof(fechaCheckIn));
            this.FechaCheckIn = fechaCheckIn;
        }

        public void SetFechaCheckOut(DateTime fechaCheckOut)
        {
            if (fechaCheckOut <= FechaCheckIn)
                throw new ArgumentException("La fecha de check-out debe ser posterior a la fecha de check-in.", nameof(fechaCheckOut));
            this.FechaCheckOut = fechaCheckOut;
        }

        public void SetEstado(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
                throw new ArgumentException("El estado de la reserva no puede ser nulo o vacío.", nameof(estado));
            this.Estado = estado;
        }
        public void SetTotal(float total)
        {
            if (total < 0)
                throw new ArgumentException("El total de la reserva no puede ser negativo.", nameof(total));
            this.Total = total;
        }

        public void SetMetodoPago(string metodoPago)
        {
            if (string.IsNullOrWhiteSpace(metodoPago))
                throw new ArgumentException("El método de pago no puede ser nulo o vacío.", nameof(metodoPago));
            this.MetodoPago = metodoPago;
        }

        public void AgregarServicio(ReservaServicio servicio)
        {
            if (servicio == null)
                throw new ArgumentNullException(nameof(servicio), "El servicio no puede ser nulo.");
            Servicios.Add(servicio);
        }

        public void EliminarServicio(ReservaServicio servicioAdicional)
        {
            if (servicioAdicional == null)
                throw new ArgumentNullException(nameof(servicioAdicional), "El servicio no puede ser nulo.");
            Servicios.Remove(servicioAdicional);
        }
        public void ActualizarServicio(ReservaServicio servicioAdicional)
        {
            if (servicioAdicional == null)
                throw new ArgumentNullException(nameof(servicioAdicional), "El servicio no puede ser nulo.");
            var index = Servicios.FindIndex(s => s.ServicioAdicional.Id == servicioAdicional.ServicioAdicional.Id);
            if (index >= 0)
            {
                Servicios[index] = servicioAdicional;
            }
            else
            {
                throw new KeyNotFoundException("El servicio no se encuentra en la reserva.");
            }
        }
        public decimal CalcularTotal()
        {
            decimal totalServicios = (decimal)Servicios.Sum(s => s.subtotal);
            return Habitacion.precio += (totalServicios);
        }
    
      
    }
}