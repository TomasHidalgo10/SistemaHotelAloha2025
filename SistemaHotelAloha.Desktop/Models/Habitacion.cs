using SistemaHotelAloha.Desktop.Utils;

namespace SistemaHotelAloha.Desktop.Models
{
    public class Habitacion
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;

        // Nuevos (BD normalizada)
        public int TipoId { get; set; }           // FK -> tipo_habitacion.Id
        public int EstadoId { get; set; }         // FK -> estado_habitacion.Id

        public decimal PrecioNoche { get; set; }

        // Opcional para mostrar en grillas si traés con JOIN
        public string? TipoNombre { get; set; }
        public string? EstadoNombre { get; set; }

        // ======== Compatibilidad hacia atrás (NO romper código viejo) ========
        [Obsolete("Usar TipoId en su lugar")]
        public string Tipo
        {
            get => LookupCache.GetTipoNombre(TipoId) ?? string.Empty;
            set
            {
                var id = LookupCache.GetTipoId(value);
                if (id.HasValue) TipoId = id.Value;
            }
        }

        [Obsolete("Usar EstadoId en su lugar")]
        public string Estado
        {
            get => LookupCache.GetEstadoNombre(EstadoId) ?? string.Empty;
            set
            {
                var id = LookupCache.GetEstadoId(value);
                if (id.HasValue) EstadoId = id.Value;
            }
        }
    }
}