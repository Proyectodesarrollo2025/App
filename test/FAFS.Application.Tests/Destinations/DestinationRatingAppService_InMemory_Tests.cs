using FAFS.Application.Contracts.Destinations;
using FAFS.Destinations;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Xunit;

namespace FAFS.Application.Tests.Destinations
{
    public class DestinationRatingAppService_InMemory_Tests : FAFSApplicationTestBase
    {
        private readonly IDestinationRatingAppService _appService;

        public DestinationRatingAppService_InMemory_Tests()
        {
            _appService = GetRequiredService<IDestinationRatingAppService>();
        }

        [Fact]
        public async Task Should_Filter_By_Current_User()
        {
            // Arrange
            var currentUser = GetRequiredService<ICurrentUser>();
            var userId = currentUser.Id ?? Guid.NewGuid();
            var destinationId = Guid.NewGuid();

            // Act
            await _appService.RateDestinationAsync(destinationId, 5, "Increíble");

            // Assert
            var repo = GetRequiredService<IRepository<DestinationRating, Guid>>();

            var ratings = await repo.GetListAsync();

            ratings.ShouldAllBe(r => r.UserId == userId);
        }
    }
}
