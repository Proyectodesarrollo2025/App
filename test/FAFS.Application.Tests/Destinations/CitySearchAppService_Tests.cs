using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Xunit;
using FAFS.Destinations;
using FAFS.Application.Contracts.Destinations;

namespace FAFS.Application.Tests.Destinations
{
    public class CitySearchAppService_Tests
    {
        private readonly Mock<ICitySearchService> _mockCitySearchService;
        private readonly Mock<IRepository<Destination, Guid>> _mockRepository;
        private readonly DestinationAppService _appService;

        public CitySearchAppService_Tests()
        {
            // 🔹 Mock del repositorio y del servicio externo
            _mockCitySearchService = new Mock<ICitySearchService>();
            _mockRepository = new Mock<IRepository<Destination, Guid>>();

            // 🔹 Inyección en el AppService real
            _appService = new DestinationAppService(_mockRepository.Object, _mockCitySearchService.Object);
        }

        [Fact]
        public async Task SearchCitiesAsync_Should_Return_Results()
        {
            // Arrange
            _mockCitySearchService
                .Setup(s => s.SearchCitiesAsync(It.IsAny<CitySearchRequestDto>()))
                .ReturnsAsync(new CitySearchResultDto
                {
                    Cities = new List<CityDto>
                    {
                        new CityDto { Name = "Río Cuarto", Country = "Argentina", CountryCode = "AR" }
                    }
                });

            var request = new CitySearchRequestDto { PartialName = "Rio" };

            // Act
            var result = await _appService.SearchCitiesAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Cities.Count.ShouldBe(1);
            result.Cities[0].Country.ShouldBe("Argentina");
        }

        [Fact]
        public async Task SearchCitiesAsync_Should_Return_Empty_When_No_Results()
        {
            // Arrange
            _mockCitySearchService
                .Setup(s => s.SearchCitiesAsync(It.IsAny<CitySearchRequestDto>()))
                .ReturnsAsync(new CitySearchResultDto { Cities = new List<CityDto>() });

            // Act
            var result = await _appService.SearchCitiesAsync(new CitySearchRequestDto
            {
                PartialName = "CiudadInexistente"
            });

            // Assert
            result.ShouldNotBeNull();
            result.Cities.ShouldBeEmpty();
        }

        [Fact]
        public async Task SearchCitiesAsync_Should_Throw_When_Input_Invalid()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "A" }; // demasiado corto

            _mockCitySearchService
                .Setup(s => s.SearchCitiesAsync(It.Is<CitySearchRequestDto>(r =>
                    string.IsNullOrWhiteSpace(r.PartialName) || r.PartialName.Length < 2)))
                .ThrowsAsync(new BusinessException("CitySearch:InvalidPartialName")
                    .WithData("Message", "The search text must contain at least 2 characters."));

            // Act & Assert
            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _appService.SearchCitiesAsync(request);
            });
        }

        [Fact]
        public async Task SearchCitiesAsync_Should_Propagate_When_Api_Fails()
        {
            // Arrange
            _mockCitySearchService
                .Setup(s => s.SearchCitiesAsync(It.IsAny<CitySearchRequestDto>()))
                .ThrowsAsync(new HttpRequestException("API not available"));

            // Act & Assert
            await Should.ThrowAsync<HttpRequestException>(async () =>
            {
                await _appService.SearchCitiesAsync(new CitySearchRequestDto
                {
                    PartialName = "Buenos Aires"
                });
            });
        }
    }
}
