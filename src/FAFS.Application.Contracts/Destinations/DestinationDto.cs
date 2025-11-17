using System;
using Volo.Abp.Application.Dtos;

namespace FAFS.Application.Contracts.Destinations
{
    public class DestinationDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
}

