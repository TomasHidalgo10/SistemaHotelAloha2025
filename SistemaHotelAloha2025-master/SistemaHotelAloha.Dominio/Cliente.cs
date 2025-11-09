using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaHotelAloha.Dominio;

namespace SistemaHotelAloha.Dominio
{
    public class Cliente : Usuario
    {
        public string Dni { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Nacionalidad { get; set; }

        public List<Reserva> HistoricoReservas { get; set; } = new List<Reserva>();

        public Cliente(int id, string nombre, string apellido, string email, string contraseña, string telefono, string dni, DateTime fechaNacimiento, string nacionalidad, DateTime fechaRegistro)
            : base(id, nombre, apellido, email, contraseña, telefono, fechaRegistro)
        {
            this.Dni = dni;
            this.FechaNacimiento = fechaNacimiento;
            this.Nacionalidad = nacionalidad;
        }

        public void SetDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("El dni no puede ser nulo o vacío.", nameof(dni));
            this.Dni = dni;
        }

        public void SetFechaNacimiento(DateTime fechaNacimiento)
        {
            if (fechaNacimiento > DateTime.Now)
                throw new ArgumentException("La fecha de nacimiento no puede ser futura.", nameof(fechaNacimiento));
            this.FechaNacimiento = fechaNacimiento;
        }

        public void SetNacionalidad(string nacionalidad)
        {
            if (string.IsNullOrWhiteSpace(nacionalidad))
                throw new ArgumentException("La nacionalidad no puede ser nula o vacía.", nameof(nacionalidad));
            this.Nacionalidad = nacionalidad;
        }
    }
}