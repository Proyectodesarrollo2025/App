using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;


namespace FAFS.Application.Contracts.Destinations
{
    public class CreateUpdateDestinationDto : AuditedEntityDto<Guid>
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        public string? PhotoUrl { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
}