using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SistemaHotelAloha.Dominio;

namespace SistemaHotelAloha.AccesoDatos.EF
{
    internal class AlohaContext : DbContext
    {
        // DbSets del dominio 
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Recepcionista> Recepcionistas { get; set; } = null!;
        public DbSet<TipoHabitacion> TiposHabitacion { get; set; } = null!;
        public DbSet<Habitacion> Habitaciones { get; set; } = null!;
        public DbSet<ServicioAdicional> ServiciosAdicionales { get; set; } = null!;
        public DbSet<Reserva> Reservas { get; set; } = null!;
        public DbSet<ReservaServicio> ReservaServicios { get; set; } = null!;
        public DbSet<Pago> Pagos { get; set; } = null!;
        public DbSet<Temporada> Temporadas { get; set; } = null!;

        internal AlohaContext()
        {
            // Arranque rápido
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                string connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            // USUARIO (TPH con Cliente/Recepcionista)
            
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Nombre).HasMaxLength(100);
                entity.Property(e => e.Apellido).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property<string>("Discriminator").HasMaxLength(50);

                entity.HasDiscriminator<string>("Discriminator")
                      .HasValue<Usuario>("Usuario")
                      .HasValue<Cliente>("Cliente")
                      .HasValue<Recepcionista>("Recepcionista");
            });

            // Campos de Cliente 
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.Property<string?>("Dni").HasMaxLength(20);
                entity.Property<string?>("Nacionalidad").HasMaxLength(60);
            });

            
            // TIPO HABITACIÓN
            
            modelBuilder.Entity<TipoHabitacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(300);
            });

            
            // HABITACIÓN (N:1 con TipoHabitacion)
            
            modelBuilder.Entity<Habitacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property<string>("Numero").IsRequired().HasMaxLength(20);
                entity.Property<decimal>("Precio").HasColumnType("decimal(18,2)");
                entity.Property<string?>("Estado").HasMaxLength(40);

                entity.HasOne(h => h.TipoHabitacion)
                      .WithMany()
                      .HasForeignKey("TipoHabitacionId")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            
            // SERVICIO ADICIONAL
            
            modelBuilder.Entity<ServicioAdicional>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property<decimal>("Precio").HasColumnType("decimal(18,2)");
            });

            
            // RESERVA (N:1 Cliente / N:1 Habitación)
            
            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FechaInicio).IsRequired();
                entity.Property(e => e.FechaFin).IsRequired();
                entity.Property<string?>("Estado").HasMaxLength(40);

                entity.HasOne(r => r.Cliente)
                      .WithMany()
                      .HasForeignKey("ClienteId")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Habitacion)
                      .WithMany()
                      .HasForeignKey("HabitacionId")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            
            // RESERVA SERVICIO
            
            modelBuilder.Entity<ReservaServicio>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne<Reserva>()
                      .WithMany()
                      .HasForeignKey("ReservaId")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<ServicioAdicional>()
                      .WithMany()
                      .HasForeignKey("ServicioAdicionalId")
                      .OnDelete(DeleteBehavior.Restrict);
            });

           
            // PAGO (N:1 con Reserva)
           
            modelBuilder.Entity<Pago>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property<decimal>("Monto").HasColumnType("decimal(18,2)");
                entity.Property(e => e.Fecha).IsRequired();
                entity.Property<string?>("Metodo").HasMaxLength(40);

                entity.HasOne<Reserva>()
                      .WithMany()
                      .HasForeignKey("ReservaId")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TEMPORADA
            modelBuilder.Entity<Temporada>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Inicio).IsRequired();
                entity.Property(e => e.Fin).IsRequired();
                entity.Property<decimal?>("FactorPrecio").HasColumnType("decimal(9,4)");
            });

        }
    }
}
