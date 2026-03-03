using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using FAFS.Experiences;
using FAFS.Destinations;

namespace FAFS.EntityFrameworkCore.Experiences
{
    public class ExperienceAppService_Tests : FAFSEntityFrameworkCoreTestBase
    {
        private readonly IExperienceAppService _experienceAppService;
        private readonly IRepository<Destination, Guid> _destinationRepository;
        private readonly FakeCurrentUser _fakeCurrentUser;

        public ExperienceAppService_Tests()
        {
            _experienceAppService = GetRequiredService<IExperienceAppService>();
            _destinationRepository = GetRequiredService<IRepository<Destination, Guid>>();
            _fakeCurrentUser = GetRequiredService<FakeCurrentUser>();
        }

        private async Task<Guid> CreateTestDestinationAsync()
        {
            var destinationId = Guid.NewGuid();
            await _destinationRepository.InsertAsync(new Destination(
                destinationId,
                "Test Destination",
                "Test Country",
                "Test City",
                "test.jpg",
                DateTime.Now,
                new Coordinates("0", "0")
            ));
            return destinationId;
        }

        [Fact]
        public async Task Should_Create_A_Valid_Experience()
        {
            // Arrange
            _fakeCurrentUser.Id = Guid.NewGuid();
            var destinationId = await CreateTestDestinationAsync();
            
            var input = new CreateUpdateExperienceDto
            {
                DestinationId = destinationId,
                Title = "Hermoso viaje a Bariloche",
                Description = "La gastronomía fue excelente y los paisajes increíbles.",
                Rating = ExperienceRating.Positiva
            };

            // Act
            var result = await _experienceAppService.CreateAsync(input);

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Title.ShouldBe(input.Title);
            result.Rating.ShouldBe(ExperienceRating.Positiva);
        }

        [Fact]
        public async Task Should_Filter_By_Rating()
        {
            // Arrange
            _fakeCurrentUser.Id = Guid.NewGuid();
            var destinationId = await CreateTestDestinationAsync();
            
            await _experienceAppService.CreateAsync(new CreateUpdateExperienceDto
            {
                DestinationId = destinationId,
                Title = "Malo",
                Description = "No me gusto",
                Rating = ExperienceRating.Negativa
            });

            await _experienceAppService.CreateAsync(new CreateUpdateExperienceDto
            {
                DestinationId = destinationId,
                Title = "Bueno",
                Description = "Me gusto",
                Rating = ExperienceRating.Positiva
            });

            // Act
            var result = await _experienceAppService.GetListAsync(new GetExperiencesInput
            {
                Rating = ExperienceRating.Negativa
            });

            // Assert
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(1);
            result.Items.All(x => x.Rating == ExperienceRating.Negativa).ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Search_By_Keyword()
        {
            // Arrange
            _fakeCurrentUser.Id = Guid.NewGuid();
            var destinationId = await CreateTestDestinationAsync();
            
            await _experienceAppService.CreateAsync(new CreateUpdateExperienceDto
            {
                DestinationId = destinationId,
                Title = "Seguridad en el viaje",
                Description = "Todo muy tranquilo",
                Rating = ExperienceRating.Positiva
            });

            await _experienceAppService.CreateAsync(new CreateUpdateExperienceDto
            {
                DestinationId = destinationId,
                Title = "Gastronomia increible",
                Description = "Todo muy rico",
                Rating = ExperienceRating.Positiva
            });

            // Act
            var result = await _experienceAppService.GetListAsync(new GetExperiencesInput
            {
                Keyword = "gastronomia"
            });

            // Assert
            result.TotalCount.ShouldBe(1);
            result.Items[0].Title.ShouldContain("Gastronomia");
        }

        [Fact]
        public async Task Should_Not_Update_Someone_Else_Experience()
        {
            // Arrange
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();

            _fakeCurrentUser.Id = user1Id;
            var destinationId = await CreateTestDestinationAsync();
            
            var experience = await _experienceAppService.CreateAsync(new CreateUpdateExperienceDto
            {
                DestinationId = destinationId,
                Title = "Mi Experiencia",
                Description = "Descripción",
                Rating = ExperienceRating.Neutral
            });

            // Switch user
            _fakeCurrentUser.Id = user2Id;

            // Act & Assert
            await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
            {
                await _experienceAppService.UpdateAsync(experience.Id, new CreateUpdateExperienceDto
                {
                    DestinationId = experience.DestinationId,
                    Title = "Hackeado",
                    Description = "Cambiando descripción ajena",
                    Rating = ExperienceRating.Negativa
                });
            });
        }
    }
}
