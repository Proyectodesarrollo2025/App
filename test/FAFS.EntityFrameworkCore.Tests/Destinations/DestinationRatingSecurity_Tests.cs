using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Destinations;
using Shouldly;
using Xunit;

using FAFS.EntityFrameworkCore;

namespace FAFS.Destinations
{
    public class DestinationRatingSecurity_Tests : FAFSEntityFrameworkCoreTestBase
    {
        [Fact]
        public async Task Should_Return_401_If_No_Token_Provided()
        {
            // Arrange
            var service = GetRequiredService<IDestinationRatingAppService>();
            var destinationId = Guid.NewGuid();

            // Act & Assert
            await Should.ThrowAsync<Volo.Abp.Authorization.AbpAuthorizationException>(async () =>
            {
                await service.RateDestinationAsync(destinationId, 5, "SinToken");
            });
        }
    }
}
