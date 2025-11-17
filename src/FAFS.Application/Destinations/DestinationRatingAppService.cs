using FAFS.Application.Contracts.Destinations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace FAFS.Destinations
{
    [Authorize] // 🔒 obliga a que el usuario tenga token JWT
    public class DestinationRatingAppService : ApplicationService, IDestinationRatingAppService
    {
        private readonly IRepository<DestinationRating, Guid> _ratingRepository;
        private readonly ICurrentUser _currentUser;

        public DestinationRatingAppService(
            IRepository<DestinationRating, Guid> ratingRepository,
            ICurrentUser currentUser)
        {
            _ratingRepository = ratingRepository;
            _currentUser = currentUser;
        }

        public async Task RateDestinationAsync(Guid destinationId, int score, string? comment)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debe estar autenticado para calificar un destino.");

            if (score < 1 || score > 5)
                throw new ArgumentException("La puntuación debe estar entre 1 y 5.");

            var rating = new DestinationRating(
                GuidGenerator.Create(),
                _currentUser.GetId(),
                destinationId,
                score,
                comment
            );

            await _ratingRepository.InsertAsync(rating, autoSave: true);
        }
    }
}
