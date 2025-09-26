// Repositorios EF con GetByCriteria implementado con LINQ (sin TODOs ni SQL manual).
// Patrón compatible con tu PersonaRepository, pero evitando mapeo manual de constructores.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaHotelAloha.Dominio;
using SistemaHotelAloha.AccesoDatos.EF;

namespace SistemaHotelAloha.AccesoDatos
{
    public class TemporadaRepository
    {

        private AlohaContext CreateContext()
        {
            return new AlohaContext(); // Usa OnConfiguring -> appsettings.json y EnsureCreated
        }

        public Temporada Add(Temporada entity)
        {
            using var context = CreateContext();
            context.Set<Temporada>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            using var context = CreateContext();
            var existing = context.Set<Temporada>().Find(id);
            if (existing != null)
            {
                context.Set<Temporada>().Remove(existing);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public Temporada? Get(int id)
        {
            using var context = CreateContext();
            return context.Set<Temporada>().Find(id);
        }

        public IEnumerable<Temporada> GetAll()
        {
            using var context = CreateContext();
            return context.Set<Temporada>().AsNoTracking().ToList();
        }

        public bool Update(Temporada entity)
        {
            using var context = CreateContext();
            var existing = context.Set<Temporada>().Find(entity.Id);
            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(entity);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public IEnumerable<Temporada> GetByCriteria(string texto)
        {
            using var context = CreateContext();
            var q = context.Set<Temporada>().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(texto)) q = q.Where(x => (x.Nombre ?? "").Contains(texto));
            return q.ToList();
        }

    }
}
