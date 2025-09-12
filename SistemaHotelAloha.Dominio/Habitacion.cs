using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.Dominio
{
    public class Habitacion
    {
        public decimal precio = 0;

        public string? Id { get; set; }
        public int Numero { get; set; }
        public TipoHabitacion TipoHabitacion { get; set; }
        public string Estado { get; set; }
        public float PrecioBase { get; set; }
        public string? Capacidad { get; set; }
        public string Descripcion { get; set; }
        public string Servicio { get; set; }

        public Habitacion (string id, int numero, TipoHabitacion tipoHabitacion, string estado, float precioBase, string capacidad, string descripcion, string servicio) 
        {
            this.Id = id;
            this.Numero = numero;
            TipoHabitacion = tipoHabitacion;
            this.Estado = estado;
            this.PrecioBase = precioBase;
            this.Capacidad = capacidad;
            this.Descripcion = descripcion;
            this.Servicio = servicio;
        }

        public void SetId (string id) 
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("El id de la habitación no puede ser nulo", nameof(id));
            this.Id = id;         
        }

        public void SetNumero(int numero) 
        {
            if(numero <= 0) 
                throw new ArgumentException("El número de la habitación debe ser mayor o igual a 0", nameof(numero));
            this.Numero = numero;
        }

        public void SetTipoHabitacion(TipoHabitacion tipoHabitacion) 
        {
            if (tipoHabitacion == null)
                throw new ArgumentException("El tipo de habitación no puede ser nulo", nameof(tipoHabitacion));
            this.TipoHabitacion= tipoHabitacion;
        }

        public void SetEstado(string estado) 
        {
            if(string.IsNullOrWhiteSpace(estado)) 
                throw new ArgumentException("El estado no puede ser nulo", nameof (estado));
            this.Estado = estado;
        }

        public void SetPrecioBase(decimal precioBase) 
        {
            if (precioBase > 0)
                throw new ArgumentException("El precio no puede ser negativo", nameof(precioBase));
            this.PrecioBase = (float)precioBase;
        }

        public void SetCapacidad(string capacidad) 
        {
            if (string.IsNullOrWhiteSpace(capacidad))
                throw new ArgumentException("La capacidad no puede ser nula", nameof(capacidad));
            this.Capacidad = capacidad;
        }

        public void SetDescripcion(string descripcion) 
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripcion no puede ser nula", nameof(descripcion));
            this.Descripcion = descripcion;
        }

        public void SetServicio(string servicio) 
        {
            if (!string.IsNullOrWhiteSpace(servicio))
                throw new ArgumentException("Servicio no puede estar vacio", nameof(servicio));
            this.Servicio = servicio;
        }

    }

}
