using FAFS.Destinations;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Validation;
using Xunit;
using FAFS.Application.Contracts.Destinations;

namespace FAFS.Application.Tests.Destinations
{
    public class DestinationAppService_Tests : FAFSApplicationTestBase
    {
        private readonly DestinationAppService _destinationAppService;
        private readonly IRepository<Destination, Guid> _destinationRepository;

        public DestinationAppService_Tests()
        {
            _destinationRepository = GetRequiredService<IRepository<Destination, Guid>>();
            _destinationAppService = GetRequiredService<DestinationAppService>();
        }

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
        public async Task Should_Throw_Exception_When_Name_Is_Empty()
        {
            var input = new CreateUpdateDestinationDto
            {
                Name = "", // inválido
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

        [Fact]
        public async Task Should_Throw_Exception_When_Name_Is_Null()
        {
            var input = new CreateUpdateDestinationDto
            {
                Name = null!, // inválido
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

        [Fact]
        public async Task Should_Throw_Exception_When_Country_Is_Empty()
        {
            var input = new CreateUpdateDestinationDto
            {
                Name = "Destino X",
                Country = "", // inválido
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

