using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaHotelAloha.Dominio;
using SistemaHotelAloha.AccesoDatos.EF;

namespace SistemaHotelAloha.AccesoDatos
{
    public class ClienteRepository
    {

        private AlohaContext CreateContext()
        {
            return new AlohaContext(); // Usa OnConfiguring -> appsettings.json y EnsureCreated
        }

        public Cliente Add(Cliente entity)
        {
            using var context = CreateContext();
            context.Set<Cliente>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            using var context = CreateContext();
            var existing = context.Set<Cliente>().Find(id);
            if (existing != null)
            {
                context.Set<Cliente>().Remove(existing);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public Cliente? Get(int id)
        {
            using var context = CreateContext();
            return context.Set<Cliente>().Find(id);
        }

        public IEnumerable<Cliente> GetAll()
        {
            using var context = CreateContext();
            return context.Set<Cliente>().AsNoTracking().ToList();
        }

        public bool Update(Cliente entity)
        {
            using var context = CreateContext();
            var existing = context.Set<Cliente>().Find(entity.Id);
            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(entity);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public IEnumerable<Cliente> GetByCriteria(string texto)
        {
            using var context = CreateContext();
            var q = context.Set<Cliente>().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(texto)) q = q.Where(x => (x.Nombre ?? "").Contains(texto) || (x.Apellido ?? "").Contains(texto) || (x.Email ?? "").Contains(texto) || (x.Telefono ?? "").Contains(texto));
            return q.ToList();
        }

    }
}
