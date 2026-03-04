using Volo.Abp.Application.Dtos;

namespace FAFS.Application.Contracts.Destinations
{
    public class GetDestinationsInput : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
    }
}
