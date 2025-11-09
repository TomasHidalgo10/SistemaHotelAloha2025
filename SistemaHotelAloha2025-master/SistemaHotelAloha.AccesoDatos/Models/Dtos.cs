using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.AccesoDatos.Models
{
    // ===== Nico: usado por el reporte mensual (PDF/tabla) =====
    public class ReservaReporteDto
    {
        public int IdReserva { get; set; }
        public string? Huesped { get; set; } = "";
        public string? HabitacionNumero { get; set; } = "";
        public string? HabitacionTipo { get; set; } = "";
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int Noches { get; set; }
        public decimal Total { get; set; }
        public string? Estado { get; set; } = "";
    }

    // ===== DTOs adicionales (compatibles con repos ADO y PDF) =====

    public class HabitacionDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; } = "";     // Ej: "Hab. 101"
        public int Capacidad { get; set; }           // Default lo resuelve la consulta (2 si es NULL)
        public decimal PrecioNoche { get; set; }     // Mapea a Habitaciones.PrecioBase
        public string? Descripcion { get; set; }
        public string? Estado { get; set; }          // "Disponible", "Ocupada", etc.
    }

    public class ReservaDTO
    {
        public int Id { get; set; }
        public string? Habitacion { get; set; } = ""; // Ej: "Hab. 101"
        public string? Destino { get; set; } = "Hotel Aloha";
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int Noches { get; set; }              // (FechaHasta - FechaDesde).Days
        public decimal PrecioNoche { get; set; }     // Habitaciones.PrecioBase
        public decimal Total { get; set; }           // Calculado o leído (PrecioTotal/Total)
        public string? Estado { get; set; } = "Confirmada";
    }

    public class ReservaDetalleParaPdf
    {
        public int Id { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string? Estado { get; set; } = "";
        public DateTime Creada { get; set; }         // Fecha de generación/creación del comprobante
        public string? Habitacion { get; set; } = ""; // Ej: "Hab. 101"
        public int Capacidad { get; set; }
        public decimal PrecioNoche { get; set; }
        public string? Destino { get; set; } = "Hotel Aloha";
        public string? DestinoDescripcion { get; set; }
        public string? Huesped { get; set; }
        public decimal Total { get; set; }
    }

    public class ServicioDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; } = "";
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        /// <summary>"PorNoche" o "PorEstadia"</summary>
        public string? PrecioTipo { get; set; } = "PorEstadia";
        public bool Activo { get; set; } = true;
    }

    public class ReservaServicioLineaDTO
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; } = "";
        public int Cantidad { get; set; } = 1;
        public decimal PrecioUnit { get; set; }
        /// <summary>"PorNoche" o "PorEstadia"</summary>
        public string? PrecioTipo { get; set; } = "PorEstadia";
        public decimal Subtotal { get; set; }         // Calculado en repo (unit * cant * mult)
    }
}