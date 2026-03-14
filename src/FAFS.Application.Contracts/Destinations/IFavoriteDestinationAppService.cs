using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using FAFS.Application.Contracts.Destinations;

namespace FAFS.Destinations
{
    public interface IFavoriteDestinationAppService : IApplicationService
    {
        Task ToggleFavoriteAsync(ToggleFavoriteDto input);
        Task<List<DestinationDto>> GetMyFavoritesAsync();
    }
}
