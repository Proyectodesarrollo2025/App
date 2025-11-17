namespace FAFS.Application.Contracts.Destinations
{
    // DTO used to send the search parameters for cities
    public class CitySearchRequestDto
    {
        public string PartialName { get; set; } = string.Empty; // Texto parcial a buscar (mínimo 2 caracteres)
        public int Limit { get; set; } = 10;                    // Límite de resultados
        public string? CountryCode { get; set; }                // Código del país opcional (ej. "AR", "UY")
    }
}
