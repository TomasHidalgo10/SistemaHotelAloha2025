using System.Text.RegularExpressions;
namespace SistemaHotelAloha.Dominio
{
    public class Usuario
    {
        public int Id { get; private set; }
        public string? Nombre { get; private set; } 
        public string? Apellido { get; private set; }
        public string? Email { get; private set; } 
        public string? Contraseña { get; private set; } 
        public string? Telefono { get; private set; } 
        public DateTime FechaRegistro { get; private set; }
        public bool Activo { get; private set; } = true;

        public Usuario(int id, string nombre, string apellido, string email, string contraseña, string telefono, DateTime fechaRegistro)
        {
            SetId(id);
            SetNombre(nombre);
            SetApellido(apellido);
            SetEmail(email);
            SetContraseña(contraseña);
            SetTelefono(telefono);
            SetFechaRegistro(fechaRegistro);
        }

        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser un número positivo.", nameof(id));
            this.Id = id;
        }

        public void SetNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre no puede ser nulo o vacío.", nameof(nombre));
            this.Nombre = nombre; 
        }

        public void SetApellido(string apellido)
        {
            if (string.IsNullOrWhiteSpace(apellido))
                throw new ArgumentException("El apellido no puede ser nulo o vacío.", nameof(apellido));
            this.Apellido = apellido; 
        }

        public void SetEmail(string email)
        {
            if (!EsEmailValido(email))
                throw new ArgumentException("El email no tiene un formato válido.", nameof(email));
            this.Email = email; 
        }

        private static bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public void SetContraseña(string contraseña)
        {
            if (string.IsNullOrWhiteSpace(contraseña))
                throw new ArgumentException("La contraseña no puede ser nula o vacía.", nameof(contraseña));
            this.Contraseña = contraseña; 
        }

        public void SetTelefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                throw new ArgumentException("El teléfono no puede ser nulo o vacío.", nameof(telefono));
            this.Telefono = telefono; 
        }

        public void SetFechaRegistro(DateTime fechaRegistro)
        {
            if (fechaRegistro == default)
                throw new ArgumentException("La fecha de registro no puede ser nula.", nameof(fechaRegistro));
            this.FechaRegistro = fechaRegistro;
        }
    }
    }
    
