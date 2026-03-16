using Volo.Abp.Application.Dtos;

namespace FAFS.Notifications
{
    public class GetAppNotificationsInput : PagedAndSortedResultRequestDto
    {
        public bool? IsRead { get; set; }
    }
}
