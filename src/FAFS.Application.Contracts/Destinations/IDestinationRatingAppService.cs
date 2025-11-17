using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FAFS.Application.Contracts.Destinations
{
    public interface IDestinationRatingAppService : IApplicationService
    {
        Task RateDestinationAsync(Guid destinationId, int score, string? comment);
    }
}
