using System;
using System.ComponentModel.DataAnnotations;

namespace FAFS.Experiences
{
    public class CreateUpdateExperienceDto
    {
        [Required]
        public Guid DestinationId { get; set; }

        [Required]
        [StringLength(ExperienceConsts.MaxTitleLength)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(ExperienceConsts.MaxDescriptionLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ExperienceRating Rating { get; set; }
    }
}
