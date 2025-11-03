using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.Desktop.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        // según tu esquema actual
        public int ClienteId { get; set; }
        public int HabitacionId { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        // 👉 Nuevo: se maneja por FK (no texto)
        public int EstadoId { get; set; }          // FK -> estado_reserva.Id
        public string? EstadoNombre { get; set; }  // (solo para mostrar en grilla)
    }
}