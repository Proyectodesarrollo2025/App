using FAFS.Application.Contracts.Destinations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FAFS.Destinations
{
    public class DestinationAppService :
     CrudAppService<
         Destination, //Entidad
         DestinationDto, //dto de salida
         Guid, //Primary key destination entity
     GetDestinationsInput, //Used for paging/sorting
     CreateUpdateDestinationDto>, //Used to create/update a destination
     IDestinationAppService //implement the IDestinationAppService
    {
        private readonly ICitySearchService _citySearchService;
        private readonly IRepository<DestinationRating, Guid> _ratingRepository;

        public DestinationAppService(
            IRepository<Destination, Guid> repository, 
            ICitySearchService citySearchService,
            IRepository<DestinationRating, Guid> ratingRepository)
            : base(repository)
        {
            _citySearchService = citySearchService;
            _ratingRepository = ratingRepository;
        }

        public override async Task<DestinationDto> GetAsync(Guid id)
        {
            var dto = await base.GetAsync(id);
            await FillRatingInfoAsync(dto);
            return dto;
        }

        public override async Task<PagedResultDto<DestinationDto>> GetListAsync(GetDestinationsInput input)
        {
            var result = await base.GetListAsync(input);
            foreach (var dto in result.Items)
            {
                await FillRatingInfoAsync(dto);
            }
            return result;
        }

        private async Task FillRatingInfoAsync(DestinationDto dto)
        {
            var ratings = await _ratingRepository.GetListAsync(r => r.DestinationId == dto.Id);
            if (ratings.Any())
            {
                dto.AverageRating = ratings.Average(r => r.Score);
                dto.RatingCount = ratings.Count;
            }
        }

        public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
        {
            return await _citySearchService.SearchCitiesAsync(request);
        }

        public async Task<CityDto?> GetCityDetailsAsync(string cityId)
        {
            return await _citySearchService.GetCityDetailsAsync(cityId);
        }

        protected override async Task<IQueryable<Destination>> CreateFilteredQueryAsync(GetDestinationsInput input)
        {
            var query = await base.CreateFilteredQueryAsync(input);

            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                query = query.Where(d => 
                    d.Name.Contains(input.Filter) || 
                    d.Country.Contains(input.Filter) ||
                    d.City.Contains(input.Filter));
            }

            return query;
        }
    }
}
