using System.Threading.Tasks;
using Shouldly;
using Xunit;
using FAFS.Destinations;
using Microsoft.Extensions.DependencyInjection;
using FAFS.Application.Contracts.Destinations;

namespace FAFS.EntityFrameworkCore.Tests.Destinations
{
    public class GeoDbCitySearchService_Integration_Tests : FAFSApplicationTestBase
    {
        [Fact(Skip = "Requiere API Key válida en user-secrets")]
        public async Task SearchCitiesAsync_Deberia_Obtener_Resultados_Reales()
        {
            // Arrange
            var service = GetRequiredService<ICitySearchService>();

            // Act
            var result = await service.SearchCitiesAsync(new CitySearchRequestDto
            {
                PartialName = "Rio",
                Limit = 5,
                CountryCode = "AR"
            });

            // Assert
            result.ShouldNotBeNull();
            result.Cities.ShouldNotBeEmpty();
        }
    }
}
