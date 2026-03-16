using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FAFS.Notifications
{
    public interface INotificationAppService : IApplicationService
    {
        Task<PagedResultDto<AppNotificationDto>> GetListAsync(GetAppNotificationsInput input);
        Task<int> GetUnreadCountAsync();
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync();
    }
}
