using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FAFS.Application.Contracts.Destinations
{
    public interface IDestinationRatingAppService : IApplicationService
    {
        Task RateDestinationAsync(Guid destinationId, int score, string? comment);
        Task UpdateRatingAsync(Guid id, int score, string? comment);
        Task DeleteRatingAsync(Guid id);
        Task<List<DestinationRatingDto>> GetRatingsAsync(Guid destinationId);
        Task<double> GetAverageRatingAsync(Guid destinationId);
        Task<List<DestinationRatingDto>> GetMyRatingsAsync();
    }

    public class DestinationRatingDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DestinationId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
