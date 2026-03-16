using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Destinations;
using FAFS.Destinations;
using Moq;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Xunit;

namespace FAFS.Application.Tests.Destinations
{
    public class DestinationRatingAppService_Unit_Tests
    {
        private readonly Mock<IRepository<DestinationRating, Guid>> _mockRatingRepository;
        private readonly Mock<ICurrentUser> _mockCurrentUser;
        private readonly Mock<Volo.Abp.Guids.IGuidGenerator> _mockGuidGenerator;
        private readonly DestinationRatingAppService _appService;

        public DestinationRatingAppService_Unit_Tests()
        {
            _mockRatingRepository = new Mock<IRepository<DestinationRating, Guid>>();
            _mockCurrentUser = new Mock<ICurrentUser>();
            _mockGuidGenerator = new Mock<Volo.Abp.Guids.IGuidGenerator>();
            _mockGuidGenerator.Setup(g => g.Create()).Returns(Guid.NewGuid());
            
            _appService = new DestinationRatingAppService(
                _mockRatingRepository.Object, 
                _mockCurrentUser.Object,
                _mockGuidGenerator.Object);
        }

        [Fact]
        public async Task RateDestinationAsync_Should_Create_Rating()
        {
            // Arrange
            var destinationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _mockCurrentUser.Setup(u => u.Id).Returns(userId);
            _mockCurrentUser.Setup(u => u.IsAuthenticated).Returns(true);
            
            _mockRatingRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<DestinationRating, bool>>>(), It.IsAny<bool>(), default))
                .ReturnsAsync(new List<DestinationRating>());

            // Act
            await _appService.RateDestinationAsync(destinationId, 5, "Great!");

            // Assert
            _mockRatingRepository.Verify(r => r.InsertAsync(It.IsAny<DestinationRating>(), true, default), Times.Once);
        }

        [Fact]
        public async Task RateDestinationAsync_Should_Throw_If_Already_Rated()
        {
            // Arrange
            var destinationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _mockCurrentUser.Setup(u => u.Id).Returns(userId);
            
            _mockRatingRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<DestinationRating, bool>>>(), It.IsAny<bool>(), default))
                .ReturnsAsync(new List<DestinationRating> { new DestinationRating(Guid.NewGuid(), userId, destinationId, 4) });

            // Act & Assert
            await Should.ThrowAsync<UserFriendlyException>(async () =>
            {
                await _appService.RateDestinationAsync(destinationId, 5, "Great!");
            });
        }

        [Fact]
        public async Task UpdateRatingAsync_Should_Update_Own_Rating()
        {
            // Arrange
            var ratingId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var rating = new DestinationRating(ratingId, userId, Guid.NewGuid(), 4);
            
            _mockCurrentUser.Setup(u => u.Id).Returns(userId);
            _mockRatingRepository.Setup(r => r.GetAsync(ratingId, true, default)).ReturnsAsync(rating);

            // Act
            await _appService.UpdateRatingAsync(ratingId, 5, "Updated comment");

            // Assert
            rating.Score.ShouldBe(5);
            rating.Comment.ShouldBe("Updated comment");
            _mockRatingRepository.Verify(r => r.UpdateAsync(rating, true, default), Times.Once);
        }

        [Fact]
        public async Task UpdateRatingAsync_Should_Throw_If_Not_Owner()
        {
            // Arrange
            var ratingId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var rating = new DestinationRating(ratingId, otherUserId, Guid.NewGuid(), 4);
            
            _mockCurrentUser.Setup(u => u.Id).Returns(userId);
            _mockRatingRepository.Setup(r => r.GetAsync(ratingId, true, default)).ReturnsAsync(rating);

            // Act & Assert
            await Should.ThrowAsync<AbpAuthorizationException>(async () =>
            {
                await _appService.UpdateRatingAsync(ratingId, 5, "Updated comment");
            });
        }

        [Fact]
        public async Task GetAverageRatingAsync_Should_Calculate_Correctly()
        {
            // Arrange
            var destinationId = Guid.NewGuid();
            var ratings = new List<DestinationRating>
            {
                new DestinationRating(Guid.NewGuid(), Guid.NewGuid(), destinationId, 5),
                new DestinationRating(Guid.NewGuid(), Guid.NewGuid(), destinationId, 3)
            };

            _mockRatingRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<DestinationRating, bool>>>(), It.IsAny<bool>(), default))
                .ReturnsAsync(ratings);

            // Act
            var result = await _appService.GetAverageRatingAsync(destinationId);

            // Assert
            result.ShouldBe(4.0);
        }

        [Fact]
        public void Authorize_Attribute_Should_Be_Present_On_Class()
        {
            var type = typeof(DestinationRatingAppService);
            var attribute = type.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);
            attribute.ShouldNotBeEmpty();
        }
    }
}
