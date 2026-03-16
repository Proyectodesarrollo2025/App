using FAFS.Application.Contracts.Destinations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace FAFS.Destinations
{
    [Authorize]
    public class DestinationRatingAppService : ApplicationService, IDestinationRatingAppService
    {
        private readonly IRepository<DestinationRating, Guid> _ratingRepository;
        private readonly ICurrentUser _currentUser;
        private readonly Volo.Abp.Guids.IGuidGenerator _guidGenerator;

        public DestinationRatingAppService(
            IRepository<DestinationRating, Guid> ratingRepository,
            ICurrentUser currentUser,
            Volo.Abp.Guids.IGuidGenerator guidGenerator)
        {
            _ratingRepository = ratingRepository;
            _currentUser = currentUser;
            _guidGenerator = guidGenerator;
        }

        public async Task RateDestinationAsync(Guid destinationId, int score, string? comment)
        {
            if (score < 1 || score > 5)
                throw new UserFriendlyException("La puntuación debe estar entre 1 y 5.");

            var ratings = await _ratingRepository.GetListAsync(r => 
                r.DestinationId == destinationId && r.UserId == _currentUser.GetId());
            
            var existingRating = ratings.FirstOrDefault();

            if (existingRating != null)
            {
                throw new UserFriendlyException("Ya has calificado este destino.");
            }

            var rating = new DestinationRating(
                _guidGenerator.Create(),
                _currentUser.GetId(),
                destinationId,
                score,
                comment
            );

            await _ratingRepository.InsertAsync(rating, autoSave: true);
        }

        public async Task UpdateRatingAsync(Guid id, int score, string? comment)
        {
            var rating = await _ratingRepository.GetAsync(id);

            if (rating.UserId != _currentUser.GetId())
            {
                throw new AbpAuthorizationException("No tienes permiso.");
            }

            rating.Score = score;
            rating.Comment = comment;

            await _ratingRepository.UpdateAsync(rating, autoSave: true);
        }

        public async Task DeleteRatingAsync(Guid id)
        {
            var rating = await _ratingRepository.GetAsync(id);

            if (rating.UserId != _currentUser.GetId())
            {
                throw new AbpAuthorizationException("No tienes permiso.");
            }

            await _ratingRepository.DeleteAsync(rating, autoSave: true);
        }

        [AllowAnonymous]
        public async Task<List<DestinationRatingDto>> GetRatingsAsync(Guid destinationId)
        {
            var ratings = await _ratingRepository.GetListAsync(r => r.DestinationId == destinationId);

            return ratings.Select(r => new DestinationRatingDto
            {
                Id = r.Id,
                UserId = r.UserId,
                DestinationId = r.DestinationId,
                Score = r.Score,
                Comment = r.Comment,
                CreationTime = r.CreationTime
            }).ToList();
        }

        [AllowAnonymous]
        public async Task<double> GetAverageRatingAsync(Guid destinationId)
        {
            var ratings = await _ratingRepository.GetListAsync(r => r.DestinationId == destinationId);
            return ratings.Any() ? ratings.Average(r => r.Score) : 0;
        }

        public async Task<List<DestinationRatingDto>> GetMyRatingsAsync()
        {
            var userId = _currentUser.Id;
            if (!userId.HasValue) return new List<DestinationRatingDto>();

            var ratings = await _ratingRepository.GetListAsync(r => r.UserId == userId.Value);

            return ratings.Select(r => new DestinationRatingDto
            {
                Id = r.Id,
                UserId = r.UserId,
                DestinationId = r.DestinationId,
                Score = r.Score,
                Comment = r.Comment,
                CreationTime = r.CreationTime
            }).ToList();
        }
    }
}
