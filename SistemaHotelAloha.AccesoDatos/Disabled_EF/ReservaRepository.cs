using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaHotelAloha.Dominio;
using SistemaHotelAloha.AccesoDatos.EF;

namespace SistemaHotelAloha.AccesoDatos
{
    public class ReservaRepository
    {

        private AlohaContext CreateContext()
        {
            return new AlohaContext(); // Usa OnConfiguring -> appsettings.json y EnsureCreated
        }

        public Reserva Add(Reserva entity)
        {
            using var context = CreateContext();
            context.Set<Reserva>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            using var context = CreateContext();
            var existing = context.Set<Reserva>().Find(id);
            if (existing != null)
            {
                context.Set<Reserva>().Remove(existing);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public Reserva? Get(int id)
        {
            using var context = CreateContext();
            return context.Set<Reserva>().Find(id);
        }

        public IEnumerable<Reserva> GetAll()
        {
            using var context = CreateContext();
            return context.Set<Reserva>().AsNoTracking().ToList();
        }

        public bool Update(Reserva entity)
        {
            using var context = CreateContext();
            var existing = context.Set<Reserva>().Find(entity.Id);
            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(entity);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public IEnumerable<Reserva> GetByCriteria(string texto)
        {
            using var context = CreateContext();
            var q = context.Set<Reserva>().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(texto)) q = q.Where(x => (x.Estado ?? "").Contains(texto));
            return q.ToList();
        }

    }
}
