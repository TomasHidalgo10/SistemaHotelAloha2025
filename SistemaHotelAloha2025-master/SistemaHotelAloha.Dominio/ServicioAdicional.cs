using SistemaHotelAloha.Dominio;
using System;

namespace SistemaHotelAloha.Dominio
{
    public class ServicioAdicional
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public float Precio { get; set; }
        public string? Descripcion { get; set; }

        public ServicioAdicional() { }

        public ServicioAdicional(int id, string nombre, float precio, string? descripcion = null)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));
            if (precio < 0)
                throw new ArgumentException("El precio no puede ser negativo.", nameof(precio));

            Id = id;
            Nombre = nombre;
            Precio = precio;
            Descripcion = descripcion;
        }

        public void SetPrecio(float precio)
        {
            if (precio < 0)
                throw new ArgumentException("El precio no puede ser negativo.", nameof(precio));
            this.Precio = precio;
        }
    }
}
