using FAFS.Notifications;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using FAFS.Application.Contracts.Destinations;
using Volo.Abp.Users;

namespace FAFS.Destinations
{
    [Authorize]
    public class FavoriteDestinationAppService : ApplicationService, IFavoriteDestinationAppService
    {
        private readonly IRepository<FavoriteDestination, Guid> _favoriteRepository;
        private readonly IRepository<Destination, Guid> _destinationRepository;
        private readonly IRepository<AppNotification, Guid> _notificationRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ICurrentUser _currentUser;

        public FavoriteDestinationAppService(
            IRepository<FavoriteDestination, Guid> favoriteRepository,
            IRepository<Destination, Guid> destinationRepository,
            IRepository<AppNotification, Guid> notificationRepository,
            IGuidGenerator guidGenerator,
            ICurrentUser currentUser)
        {
            _favoriteRepository = favoriteRepository;
            _destinationRepository = destinationRepository;
            _notificationRepository = notificationRepository;
            _guidGenerator = guidGenerator;
            _currentUser = currentUser;
        }

        // We shadow the base.CurrentUser to use our injected one
        protected new ICurrentUser CurrentUser => _currentUser;

        public async Task ToggleFavoriteAsync(ToggleFavoriteDto input)
        {
            if (CurrentUser.Id == null)
            {
                throw new UnauthorizedAccessException();
            }

            var userId = CurrentUser.Id.Value;
            var destinationId = input.DestinationId;

            var destination = await _destinationRepository.FindAsync(destinationId);
            if (destination == null)
            {
                throw new UserFriendlyException("El destino no existe.");
            }

            var existing = await _favoriteRepository.FindAsync(f => f.UserId == userId && f.DestinationId == destinationId);

            if (existing != null)
            {
                // Remove favorite
                await _favoriteRepository.DeleteAsync(existing, autoSave: true);
            }
            else
            {
                // Add favorite
                var newFavorite = new FavoriteDestination(
                    _guidGenerator.Create(),
                    userId,
                    destinationId
                );

                await _favoriteRepository.InsertAsync(newFavorite, autoSave: true);

                // Notification 1
                await _notificationRepository.InsertAsync(new AppNotification(
                    _guidGenerator.Create(),
                    userId,
                    "Destino guardado",
                    $"El destino '{destination.Name}' ha sido guardado en tus favoritos.",
                    "FavoriteAdded"
                ), autoSave: true);
            }
        }

        public async Task<List<DestinationDto>> GetMyFavoritesAsync()
        {
            if (CurrentUser.Id == null)
            {
                return new List<DestinationDto>();
            }

            var userId = CurrentUser.Id.Value;

            var query = from fav in await _favoriteRepository.GetQueryableAsync()
                        join dest in await _destinationRepository.GetQueryableAsync()
                        on fav.DestinationId equals dest.Id
                        where fav.UserId == userId
                        select dest;

            var destinations = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<Destination>, List<DestinationDto>>(destinations);
        }
    }
}
