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
    public class UsuarioRepository
    {

        private AlohaContext CreateContext()
        {
            return new AlohaContext(); // Usa OnConfiguring -> appsettings.json y EnsureCreated
        }

        public Usuario Add(Usuario entity)
        {
            using var context = CreateContext();
            context.Set<Usuario>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            using var context = CreateContext();
            var existing = context.Set<Usuario>().Find(id);
            if (existing != null)
            {
                context.Set<Usuario>().Remove(existing);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public Usuario? Get(int id)
        {
            using var context = CreateContext();
            return context.Set<Usuario>().Find(id);
        }

        public IEnumerable<Usuario> GetAll()
        {
            using var context = CreateContext();
            return context.Set<Usuario>().AsNoTracking().ToList();
        }

        public bool Update(Usuario entity)
        {
            using var context = CreateContext();
            var existing = context.Set<Usuario>().Find(entity.Id);
            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(entity);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public IEnumerable<Usuario> GetByCriteria(string texto)
        {
            using var context = CreateContext();
            var q = context.Set<Usuario>().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(texto)) q = q.Where(x => (x.Nombre ?? "").Contains(texto) || (x.Apellido ?? "").Contains(texto) || (x.Email ?? "").Contains(texto) || (x.Telefono ?? "").Contains(texto));
            return q.ToList();
        }

    }
}
