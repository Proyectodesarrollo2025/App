using System;
using Volo.Abp.Application.Services;

namespace FAFS.Experiences
{
    public interface IExperienceAppService : 
        ICrudAppService<
            ExperienceDto, 
            Guid, 
            GetExperiencesInput, 
            CreateUpdateExperienceDto>
    {
    }
}
