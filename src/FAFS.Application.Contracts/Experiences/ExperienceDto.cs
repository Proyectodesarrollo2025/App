using System;
using Volo.Abp.Application.Dtos;

namespace FAFS.Experiences
{
    public class ExperienceDto : AuditedEntityDto<Guid>
    {
        public Guid DestinationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ExperienceRating Rating { get; set; }
        public string? CreatorUserName { get; set; } // Opcional, para mostrar quién lo creó
    }
}
