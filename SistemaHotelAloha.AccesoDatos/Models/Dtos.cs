// SistemaHotelAloha.AccesoDatos/Models/Dtos.cs
using System;

namespace SistemaHotelAloha.AccesoDatos.Models
{
    public class HabitacionDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public int Capacidad { get; set; }
        public decimal PrecioNoche { get; set; }
        public string? Descripcion { get; set; }
        public string? Estado { get; set; }
    }

    public class ReservaDTO
    {
        public int Id { get; set; }
        public string Habitacion { get; set; } = "";
        public string Destino { get; set; } = "Hotel Aloha"; // sitio de 1 hotel
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int Noches { get; set; }
        public decimal PrecioNoche { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Confirmada";
    }

    public class ReservaDetalleParaPdf
    {
        public int Id { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string Estado { get; set; } = "";
        public DateTime Creada { get; set; }
        public string Habitacion { get; set; } = "";
        public int Capacidad { get; set; }
        public decimal PrecioNoche { get; set; }
        public string Destino { get; set; } = "Hotel Aloha";
        public string? DestinoDescripcion { get; set; }
        public string? Huesped { get; set; }
        public decimal Total { get; set; }
    }

    public class ServicioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
       
        public string PrecioTipo { get; set; } = "PorEstadia";
        public bool Activo { get; set; } = true;
    }

    public class ReservaServicioLineaDTO
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; } = "";
        public int Cantidad { get; set; } = 1;
        public decimal PrecioUnit { get; set; }
        /// <summary>"PorNoche" o "PorEstadia"</summary>
        public string PrecioTipo { get; set; } = "PorEstadia";
        public decimal Subtotal { get; set; }
    }
}
