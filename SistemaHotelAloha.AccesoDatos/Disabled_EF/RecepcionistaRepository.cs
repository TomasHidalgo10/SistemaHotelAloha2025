using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaHotelAloha.Dominio;
using SistemaHotelAloha.AccesoDatos.EF;

namespace SistemaHotelAloha.AccesoDatos
{
    public class RecepcionistaRepository
    {

        private AlohaContext CreateContext()
        {
            return new AlohaContext(); // Usa OnConfiguring -> appsettings.json y EnsureCreated
        }

        public Recepcionista Add(Recepcionista entity)
        {
            using var context = CreateContext();
            context.Set<Recepcionista>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            using var context = CreateContext();
            var existing = context.Set<Recepcionista>().Find(id);
            if (existing != null)
            {
                context.Set<Recepcionista>().Remove(existing);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public Recepcionista? Get(int id)
        {
            using var context = CreateContext();
            return context.Set<Recepcionista>().Find(id);
        }

        public IEnumerable<Recepcionista> GetAll()
        {
            using var context = CreateContext();
            return context.Set<Recepcionista>().AsNoTracking().ToList();
        }

        public bool Update(Recepcionista entity)
        {
            using var context = CreateContext();
            var existing = context.Set<Recepcionista>().Find(entity.Id);
            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(entity);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public IEnumerable<Recepcionista> GetByCriteria(string texto)
        {
            using var context = CreateContext();
            var q = context.Set<Recepcionista>().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(texto)) q = q.Where(x => (x.Nombre ?? "").Contains(texto) || (x.Apellido ?? "").Contains(texto) || (x.Email ?? "").Contains(texto) || (x.Telefono ?? "").Contains(texto));
            return q.ToList();
        }

    }
}