using System;
using System.Linq;
using System.Threading.Tasks;
using FAFS.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FAFS.Experiences
{
    [Authorize]
    public class ExperienceAppService : 
        CrudAppService<
            Experience, 
            ExperienceDto, 
            Guid, 
            GetExperiencesInput, 
            CreateUpdateExperienceDto>,
        IExperienceAppService
    {
        public ExperienceAppService(IRepository<Experience, Guid> repository) 
            : base(repository)
        {
        }

        public override async Task<ExperienceDto> UpdateAsync(Guid id, CreateUpdateExperienceDto input)
        {
            var experience = await Repository.GetAsync(id);
            
            if (experience.CreatorId != CurrentUser.Id)
            {
                throw new UnauthorizedAccessException("You can only edit your own experiences.");
            }

            return await base.UpdateAsync(id, input);
        }

        public override async Task DeleteAsync(Guid id)
        {
            var experience = await Repository.GetAsync(id);

            if (experience.CreatorId != CurrentUser.Id)
            {
                throw new UnauthorizedAccessException("You can only delete your own experiences.");
            }

            await base.DeleteAsync(id);
        }

        [AllowAnonymous]
        public override Task<ExperienceDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }

        [AllowAnonymous]
        public override Task<PagedResultDto<ExperienceDto>> GetListAsync(GetExperiencesInput input)
        {
            return base.GetListAsync(input);
        }

        protected override async Task<IQueryable<Experience>> CreateFilteredQueryAsync(GetExperiencesInput input)
        {
            var query = await base.CreateFilteredQueryAsync(input);

            return query
                .WhereIf(input.DestinationId.HasValue, x => x.DestinationId == input.DestinationId)
                .WhereIf(input.Rating.HasValue, x => x.Rating == input.Rating)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => 
                    x.Title.ToLower().Contains(input.Keyword!.ToLower()) || 
                    x.Description.ToLower().Contains(input.Keyword!.ToLower()));
        }
    }
}
