using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.Dominio
{
    public class Pago
    {
        public int Id { get; set; }
        public float Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string Metodo { get; set; }
        public string Estado { get; set; }
   
        public Pago(int id, float monto, DateTime fechaPago, string metodo, string estado)
        {
            this.Id = id;
            this.Monto = monto;
            this.FechaPago = fechaPago;
            this.Metodo = metodo;
            this.Estado = estado;
        }
    
        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del pago debe ser un número positivo.", nameof(id));
            this.Id = id;
        }   
    
        public void SetMonto(float monto)
        {
            if (monto < 0)
                throw new ArgumentException("El monto del pago no puede ser negativo.", nameof(monto));
            this.Monto = monto;
        }
    
        public void SetFechaPago(DateTime fechaPago)
        {
            if (fechaPago > DateTime.Now)
                throw new ArgumentException("La fecha del pago no puede ser futura.", nameof(fechaPago));
            this.FechaPago = fechaPago;
        }
    
      public void SetMetodo(string metodo)
        {
            if (string.IsNullOrWhiteSpace(metodo))
                throw new ArgumentException("El método de pago no puede ser nulo o vacío.", nameof(metodo));
            this.Metodo = metodo;
        }
    
        public void SetEstado(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
                throw new ArgumentException("El estado del pago no puede ser nulo o vacío.", nameof(estado));
            this.Estado = estado;
        }
    
    
       
    }

}
