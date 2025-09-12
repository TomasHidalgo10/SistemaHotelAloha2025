using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.Dominio
{
    public class ReservaServicio
    {
        public int cantidad { get; set; }
        public float subtotal { get; set; }

        // Relación 1..1 con ServicioAdicional
        public ServicioAdicional ServicioAdicional { get; set; } = null!;
        public ServicioAdicional Servicio
        {
            get => ServicioAdicional;
            set => ServicioAdicional = value;
        }

        public ReservaServicio(int cantidad, float subtotal, ServicioAdicional servicio)
        {
            this.cantidad = cantidad;
            this.subtotal = subtotal;
            this.Servicio = servicio;
        }

        public ReservaServicio()
        {
        }

        public void SetCantidad(int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero.", nameof(cantidad));
            this.cantidad = cantidad;
        }

        public void SetSubtotal(float subtotal)
        {
            if (subtotal < 0)
                throw new ArgumentException("El subtotal no puede ser negativo.", nameof(subtotal));
            this.subtotal = subtotal;
        }

        public void SetServicio(ServicioAdicional servicio)
        {
            if (servicio == null)
                throw new ArgumentNullException(nameof(servicio), "El servicio no puede ser nulo.");
            this.Servicio = servicio;
        }

        public void SetServicio(object servicioAdicional)
        {
            if (servicioAdicional is ServicioAdicional s)
            {
                this.Servicio = s;
            }
            else
            {
                throw new ArgumentException("El servicio proporcionado no es del tipo esperado.", nameof(servicioAdicional));
            }
        }

        public void SetSubtotal(object value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            try
            {
                if (value is float f)
                    this.subtotal = f;
                else if (value is double d)
                    this.subtotal = (float)d;
                else if (value is int i)
                    this.subtotal = i;
                else if (value is string s && float.TryParse(s, out var parsed))
                    this.subtotal = parsed;
                else
                    throw new ArgumentException("Tipo no soportado para subtotal.", nameof(value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException("No se pudo asignar el subtotal.", nameof(value), ex);
            }
        }
    }
}
