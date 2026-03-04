using FAFS.Destinations;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Validation;
using Xunit;
using FAFS.Application.Contracts.Destinations;
using FAFS.EntityFrameworkCore;
using System.Linq;

namespace FAFS.Destinations
{
    public class DestinationAppService_Tests : FAFSEntityFrameworkCoreTestBase
    {
        private readonly IDestinationAppService _destinationAppService;
        private readonly IRepository<Destination, Guid> _destinationRepository;

        public DestinationAppService_Tests()
        {
            _destinationRepository = GetRequiredService<IRepository<Destination, Guid>>();
            _destinationAppService = GetRequiredService<IDestinationAppService>();
        }

        // 3.1 & 3.2: Buscar ciudades por nombre y filtros (Usando Mock registro en TestModule)
        [Fact]
        public async Task SearchCitiesAsync_Should_Work_With_PartialName()
        {
            var input = new CitySearchRequestDto
            {
                PartialName = "Rio",
                Limit = 5
            };

            var result = await _destinationAppService.SearchCitiesAsync(input);

            result.ShouldNotBeNull();
            result.Cities.ShouldNotBeEmpty();
            result.Cities.Any(c => c.Name.Contains("Test")).ShouldBeTrue();
        }

        [Fact]
        public async Task SearchCitiesAsync_Should_Work_With_AdvancedFilters()
        {
            var input = new CitySearchRequestDto
            {
                PartialName = "Rio",
                CountryCode = "AR",
                RegionCode = "B",
                MinPopulation = 1000000,
                Limit = 5
            };

            var result = await _destinationAppService.SearchCitiesAsync(input);

            result.ShouldNotBeNull();
            result.Cities.ShouldNotBeEmpty();
        }

        // 3.3: Obtener información detallada (Usando Mock registro en TestModule)
        [Fact]
        public async Task GetCityDetailsAsync_Should_Return_Details()
        {
            var result = await _destinationAppService.GetCityDetailsAsync("test1");

            result.ShouldNotBeNull();
            result.Id.ShouldBe("test1");
            result.Name.ShouldBe("Test City");
            result.Population.ShouldBe(100000);
        }

        // 3.5: Guardar destinos en la base interna
        [Fact]
        public async Task Should_Create_Destination_Successfully()
        {
            var input = new CreateUpdateDestinationDto
            {
                Name = "Cataratas del Iguazú",
                Country = "Argentina",
                City = "Puerto Iguazú",
                PhotoUrl = "http://example.com/photo.jpg",
                Latitude = "-25.6953",
                Longitude = "-54.4367"
            };

            var result = await _destinationAppService.CreateAsync(input);

            result.ShouldNotBeNull();
            result.Name.ShouldBe("Cataratas del Iguazú");

            var entity = await _destinationRepository.GetAsync(result.Id);
            entity.Name.ShouldBe("Cataratas del Iguazú");
        }

        [Fact]
        public async Task Should_Get_Filtered_Local_Destinations()
        {
            // Seed
            await _destinationRepository.InsertAsync(new Destination(Guid.NewGuid(), "Iguazú", "Argentina", "Puerto Iguazú", "url", DateTime.Now, new Coordinates("-25", "-54")));
            await _destinationRepository.InsertAsync(new Destination(Guid.NewGuid(), "Paris", "France", "Paris", "url", DateTime.Now, new Coordinates("48", "2")));

            var result = await _destinationAppService.GetListAsync(new GetDestinationsInput
            {
                Filter = "Iguazú"
            });

            result.TotalCount.ShouldBeGreaterThanOrEqualTo(1);
            result.Items.Any(i => i.Name == "Iguazú").ShouldBeTrue();
            result.Items.Any(i => i.Name == "Paris").ShouldBeFalse();
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Name_Is_Empty()
        {
            var input = new CreateUpdateDestinationDto
            {
                Name = "",
                Country = "Argentina",
                City = "Buenos Aires",
                PhotoUrl = "http://example.com/photo.jpg",
                Latitude = "-34.6037",
                Longitude = "-58.3816"
            };

            await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                await _destinationAppService.CreateAsync(input);
            });
        }
    }
}
