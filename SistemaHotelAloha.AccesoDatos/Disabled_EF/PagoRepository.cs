using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaHotelAloha.Dominio;
using SistemaHotelAloha.AccesoDatos.EF;

namespace SistemaHotelAloha.AccesoDatos
{
    public class PagoRepository
    {

        private AlohaContext CreateContext()
        {
            return new AlohaContext(); // Usa OnConfiguring -> appsettings.json y EnsureCreated
        }

        public Pago Add(Pago entity)
        {
            using var context = CreateContext();
            context.Set<Pago>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            using var context = CreateContext();
            var existing = context.Set<Pago>().Find(id);
            if (existing != null)
            {
                context.Set<Pago>().Remove(existing);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public Pago? Get(int id)
        {
            using var context = CreateContext();
            return context.Set<Pago>().Find(id);
        }

        public IEnumerable<Pago> GetAll()
        {
            using var context = CreateContext();
            return context.Set<Pago>().AsNoTracking().ToList();
        }

        public bool Update(Pago entity)
        {
            using var context = CreateContext();
            var existing = context.Set<Pago>().Find(entity.Id);
            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(entity);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public IEnumerable<Pago> GetByCriteria(string texto)
        {
            using var context = CreateContext();
            var q = context.Set<Pago>().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(texto)) q = q.Where(x => (x.Metodo ?? "").Contains(texto) || (x.Estado ?? "").Contains(texto));
            return q.ToList();
        }

    }
}
