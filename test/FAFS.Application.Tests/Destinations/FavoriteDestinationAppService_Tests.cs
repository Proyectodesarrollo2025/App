using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Destinations;
using FAFS.Notifications;
using Moq;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;
using Xunit;

namespace FAFS.Destinations
{
    public class FavoriteDestinationAppService_Unit_Tests
    {
        private readonly Mock<IRepository<FavoriteDestination, Guid>> _mockFavoriteRepository;
        private readonly Mock<IRepository<Destination, Guid>> _mockDestinationRepository;
        private readonly Mock<IRepository<AppNotification, Guid>> _mockNotificationRepository;
        private readonly Mock<IGuidGenerator> _mockGuidGenerator;
        private readonly Mock<ICurrentUser> _mockCurrentUser;
        private readonly FavoriteDestinationAppService _appService;

        public FavoriteDestinationAppService_Unit_Tests()
        {
            _mockFavoriteRepository = new Mock<IRepository<FavoriteDestination, Guid>>();
            _mockDestinationRepository = new Mock<IRepository<Destination, Guid>>();
            _mockNotificationRepository = new Mock<IRepository<AppNotification, Guid>>();
            _mockGuidGenerator = new Mock<IGuidGenerator>();
            _mockCurrentUser = new Mock<ICurrentUser>();

            // Setup Mock User
            _mockCurrentUser.Setup(u => u.Id).Returns(Guid.NewGuid());
            _mockCurrentUser.Setup(u => u.IsAuthenticated).Returns(true);

            _appService = new FavoriteDestinationAppService(
                _mockFavoriteRepository.Object,
                _mockDestinationRepository.Object,
                _mockNotificationRepository.Object,
                _mockGuidGenerator.Object,
                _mockCurrentUser.Object
            );
        }

        [Fact]
        public async Task ToggleFavoriteAsync_Should_Add_And_Notify_If_Not_Exists()
        {
            // Arrange
            var destId = Guid.NewGuid();
            var destination = new Destination(
                destId, 
                "Test", 
                "Country", 
                "City", 
                "url", 
                DateTime.Now, 
                new Coordinates("10", "20")
            );

            _mockDestinationRepository.Setup(r => r.FindAsync(destId, true, default))
                .ReturnsAsync(destination);

            _mockFavoriteRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<FavoriteDestination, bool>>>(), true, default))
                .ReturnsAsync((FavoriteDestination)null);

            // Act
            await _appService.ToggleFavoriteAsync(destId);

            // Assert
            _mockFavoriteRepository.Verify(r => r.InsertAsync(It.IsAny<FavoriteDestination>(), true, default), Times.Once);
            _mockNotificationRepository.Verify(r => r.InsertAsync(It.IsAny<AppNotification>(), true, default), Times.Once);
        }

        [Fact]
        public async Task ToggleFavoriteAsync_Should_Remove_If_Exists()
        {
            // Arrange
            var destId = Guid.NewGuid();
            var favorite = new FavoriteDestination(Guid.NewGuid(), _mockCurrentUser.Object.Id.Value, destId);

            var destination = new Destination(
                destId, 
                "Test", 
                "Country", 
                "City", 
                "url", 
                DateTime.Now, 
                new Coordinates("10", "20")
            );

            _mockDestinationRepository.Setup(r => r.FindAsync(destId, true, default))
                .ReturnsAsync(destination);

            _mockFavoriteRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<FavoriteDestination, bool>>>(), true, default))
                .ReturnsAsync(favorite);

            // Act
            await _appService.ToggleFavoriteAsync(destId);

            // Assert
            _mockFavoriteRepository.Verify(r => r.DeleteAsync(favorite, true, default), Times.Once);
            _mockNotificationRepository.Verify(r => r.InsertAsync(It.IsAny<AppNotification>(), true, default), Times.Never);
        }
    }
}
