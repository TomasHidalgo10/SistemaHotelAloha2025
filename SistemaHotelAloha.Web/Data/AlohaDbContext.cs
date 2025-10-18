using Microsoft.Data.SqlClient;
using SistemaHotelAloha.Dominio;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;


namespace SistemaHotelAloha.Web.Data;

public class AlohaDbContext : DbContext
{
    public AlohaDbContext(DbContextOptions<AlohaDbContext> options) : base(options) { }

    // Tablas (DbSet) que correspondan a las entidades
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Habitacion> Habitaciones { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<ServicioAdicional> ServiciosAdicionales { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
}

