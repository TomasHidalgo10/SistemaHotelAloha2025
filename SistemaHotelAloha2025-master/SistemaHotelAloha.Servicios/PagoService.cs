using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaHotelAloha.AccesoDatos;
using SistemaHotelAloha.Dominio;

namespace SistemaHotelAloha.Servicios
{
    public class PagoService
    {
        public void Add(Pago pago) 
        {
            pago.Id = PagoInMemory.Pagos.Count > 0 ? PagoInMemory.Pagos.Max(p => p.Id) + 1 : 1; 
            PagoInMemory.Pagos.Add(pago);
        }

        public List<Pago> GetAll()
        {
            return PagoInMemory.Pagos.ToList();
        }

        public Pago GetById(int id)
        {
            return PagoInMemory.Pagos.Find(p => p.Id == id)
                ?? throw new KeyNotFoundException($"Pago con ID {id} no encontrado.");
        }
        public bool Delete(int id) 
        {
            var pago = PagoInMemory.Pagos.Find(p => p.Id == id);
            if (pago != null) 
            {
                PagoInMemory.Pagos.Remove(pago);
                return true;
            }
            return false;
        }

        public void Uptade(Pago pagoActualizado) 
        {
            var index = PagoInMemory.Pagos.FindIndex(p => p.Id == pagoActualizado.Id);
            if (index == -1)
                throw new KeyNotFoundException($"Pago con ID {pagoActualizado.Id} no encontrado.");

            PagoInMemory.Pagos[index] = pagoActualizado;
        }
    }
}
