namespace FAFS.Application.Contracts.Destinations
{
    // DTO that represents a single city result from the GeoDB API
    public class CityDto
    {
        public string Name { get; set; } = string.Empty;        // Nombre de la ciudad
        public string Country { get; set; } = string.Empty;     // Nombre del país
        public string CountryCode { get; set; } = string.Empty; // Código ISO del país
        public string? Region { get; set; }                     // Región o estado
        public string? Latitude { get; set; }                   // Latitud (string por compatibilidad)
        public string? Longitude { get; set; }                  // Longitud (string por compatibilidad)
    }
}
