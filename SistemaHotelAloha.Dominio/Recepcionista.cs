using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaHotelAloha.Dominio;

namespace SistemaHotelAloha.Dominio
{
    public class Recepcionista : Usuario
    {
        public string Legajo { get; set; }
        public DateTime FechaContratacion { get; set; }
        public string Turno { get; set; }

        public Recepcionista(int id, string nombre, string apellido, string email, string contraseña, string telefono, string legajo, DateTime fechaContratacion, string turno, DateTime fechaRegistro)
            : base(id, nombre, apellido, email, contraseña, telefono, fechaRegistro)
        {
            this.Legajo = legajo;
            this.FechaContratacion = fechaContratacion;
            this.Turno = turno;
        }

        public void SetLegajo(string legajo)
        {
            if (string.IsNullOrWhiteSpace(legajo))
                throw new ArgumentException("El legajo no puede ser nulo o vacío.", nameof(legajo));
            this.Legajo = legajo;
        }

        public void SetFechaContratacion(DateTime fechaContratacion)
        {
            if (fechaContratacion > DateTime.Now)
                throw new ArgumentException("La fecha de contratación no puede ser futura.", nameof(fechaContratacion));
            this.FechaContratacion = fechaContratacion;
        }

        public void SetTurno(string turno)
        {
            if (string.IsNullOrWhiteSpace(turno))
                throw new ArgumentException("El turno no puede ser nulo o vacío.", nameof(turno));
            this.Turno = turno;
        }
    }
}
