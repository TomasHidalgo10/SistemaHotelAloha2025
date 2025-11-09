using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaHotelAloha.Dominio;
using SistemaHotelAloha.AccesoDatos.EF;

namespace SistemaHotelAloha.AccesoDatos
{
    public class ReservaServicioRepository
    {

        private AlohaContext CreateContext()
        {
            return new AlohaContext(); // Usa OnConfiguring -> appsettings.json y EnsureCreated
        }

        public ReservaServicio Add(ReservaServicio entity)
        {
            using var context = CreateContext();
            context.Set<ReservaServicio>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            using var context = CreateContext();
            var existing = context.Set<ReservaServicio>().Find(id);
            if (existing != null)
            {
                context.Set<ReservaServicio>().Remove(existing);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public ReservaServicio? Get(int id)
        {
            using var context = CreateContext();
            return context.Set<ReservaServicio>().Find(id);
        }

        public IEnumerable<ReservaServicio> GetAll()
        {
            using var context = CreateContext();
            return context.Set<ReservaServicio>().AsNoTracking().ToList();
        }

        public bool Update(ReservaServicio entity)
        {
            using var context = CreateContext();
            var existing = context.Set<ReservaServicio>().Find(entity.Id);
            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(entity);
                context.SaveChanges();
                return true;
            }
            return false;
        }

    }
}
