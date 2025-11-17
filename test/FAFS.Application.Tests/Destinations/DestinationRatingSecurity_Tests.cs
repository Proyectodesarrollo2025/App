using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FAFS.Application.Tests.Destinations
{
    public class DestinationRatingSecurity_Tests : FAFSApplicationTestBase
    {
        [Fact]
        public async Task Should_Return_401_If_No_Token_Provided()
        {
            // Arrange
            var client = GetRequiredService<IHttpClientFactory>().CreateClient();
            var destinationId = "00000000-0000-0000-0000-000000000001";

            // Act
            var response = await client.PostAsync(
                $"/api/app/destination-rating/rate-destination/{destinationId}?score=5&comment=SinToken",
                null
            );

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}
