using System.Collections.Generic;

namespace FAFS.Application.Contracts.Destinations
{
    // DTO that represents the list of city results returned by the search
    public class CitySearchResultDto
    {
        public List<CityDto> Cities { get; set; } = new(); // Lista de ciudades encontradas
    }
}
