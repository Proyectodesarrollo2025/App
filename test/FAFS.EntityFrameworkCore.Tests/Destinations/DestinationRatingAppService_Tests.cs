using FAFS.Application.Contracts.Destinations;
using FAFS.Destinations;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Xunit;
using FAFS.EntityFrameworkCore;

namespace FAFS.Destinations
{
    public class DestinationRatingAppService_Tests : FAFSEntityFrameworkCoreTestBase
    {
        private readonly IDestinationRatingAppService _appService;

        public DestinationRatingAppService_Tests()
        {
            _appService = GetRequiredService<IDestinationRatingAppService>();
        }

        [Fact]
        public async Task Should_Filter_By_Current_User()
        {
            // Arrange
            var fakeCurrentUser = (FakeCurrentUser)GetRequiredService<ICurrentUser>();
            var userId = Guid.NewGuid();
            fakeCurrentUser.Id = userId;
            var destinationId = Guid.NewGuid();

            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                await _appService.RateDestinationAsync(destinationId, 5, "Increíble");

                // Assert
                var repo = GetRequiredService<IRepository<DestinationRating, Guid>>();
                var ratings = await repo.GetListAsync();
                ratings.ShouldAllBe(r => r.UserId == userId);
            });
        }
    }
}
