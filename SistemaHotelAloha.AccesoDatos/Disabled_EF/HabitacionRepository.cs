
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaHotelAloha.Dominio;
using SistemaHotelAloha.AccesoDatos.EF;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaHotelAloha.Dominio;
using SistemaHotelAloha.AccesoDatos.EF;

namespace SistemaHotelAloha.AccesoDatos
{
    public class HabitacionRepository
    {

        private AlohaContext CreateContext()
        {
            return new AlohaContext(); // Usa OnConfiguring -> appsettings.json y EnsureCreated
        }

        public Habitacion Add(Habitacion entity)
        {
            using var context = CreateContext();
            context.Set<Habitacion>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            using var context = CreateContext();
            var existing = context.Set<Habitacion>().Find(id);
            if (existing != null)
            {
                context.Set<Habitacion>().Remove(existing);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public Habitacion? Get(int id)
        {
            using var context = CreateContext();
            return context.Set<Habitacion>().Find(id);
        }

        public IEnumerable<Habitacion> GetAll()
        {
            using var context = CreateContext();
            return context.Set<Habitacion>().AsNoTracking().ToList();
        }

        public bool Update(Habitacion entity)
        {
            using var context = CreateContext();
            var existing = context.Set<Habitacion>().Find(entity.Id);
            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(entity);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public IEnumerable<Habitacion> GetByCriteria(string texto)
        {
            using var context = CreateContext();
            var q = context.Set<Habitacion>().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(texto)) q = q.Where(x => EF.Functions.Like(EF.Property<string>(x, "Numero"), "%" + texto + "%") || EF.Functions.Like(EF.Property<string>(x, "Estado"), "%" + texto + "%"));
            return q.ToList();
        }

    }
}
