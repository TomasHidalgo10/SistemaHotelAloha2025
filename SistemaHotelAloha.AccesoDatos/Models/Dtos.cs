using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.AccesoDatos.Models;

public class ReservaReporteDto
{
    public int IdReserva { get; set; }
    public string Huesped { get; set; } = "";
    public string HabitacionNumero { get; set; } = "";
    public string HabitacionTipo { get; set; } = "";
    public DateTime FechaDesde { get; set; }
    public DateTime FechaHasta { get; set; }
    public int Noches { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "";
}
