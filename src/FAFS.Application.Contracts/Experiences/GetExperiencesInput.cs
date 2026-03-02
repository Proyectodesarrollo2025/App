using Volo.Abp.Application.Dtos;
using System;

namespace FAFS.Experiences
{
    public class GetExperiencesInput : PagedAndSortedResultRequestDto
    {
        public Guid? DestinationId { get; set; }
        public ExperienceRating? Rating { get; set; }
        public string? Keyword { get; set; }
    }
}
