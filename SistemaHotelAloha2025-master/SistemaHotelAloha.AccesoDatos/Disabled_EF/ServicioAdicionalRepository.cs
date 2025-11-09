using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaHotelAloha.Dominio;
using SistemaHotelAloha.AccesoDatos.EF;

namespace SistemaHotelAloha.AccesoDatos
{
    public class ServicioAdicionalRepository
    {

        private AlohaContext CreateContext()
        {
            return new AlohaContext(); // Usa OnConfiguring -> appsettings.json y EnsureCreated
        }

        public ServicioAdicional Add(ServicioAdicional entity)
        {
            using var context = CreateContext();
            context.Set<ServicioAdicional>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            using var context = CreateContext();
            var existing = context.Set<ServicioAdicional>().Find(id);
            if (existing != null)
            {
                context.Set<ServicioAdicional>().Remove(existing);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public ServicioAdicional? Get(int id)
        {
            using var context = CreateContext();
            return context.Set<ServicioAdicional>().Find(id);
        }

        public IEnumerable<ServicioAdicional> GetAll()
        {
            using var context = CreateContext();
            return context.Set<ServicioAdicional>().AsNoTracking().ToList();
        }

        public bool Update(ServicioAdicional entity)
        {
            using var context = CreateContext();
            var existing = context.Set<ServicioAdicional>().Find(entity.Id);
            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(entity);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public IEnumerable<ServicioAdicional> GetByCriteria(string texto)
        {
            using var context = CreateContext();
            var q = context.Set<ServicioAdicional>().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(texto)) q = q.Where(x => (x.Nombre ?? "").Contains(texto));
            return q.ToList();
        }

    }
}
