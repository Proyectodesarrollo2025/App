using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace FAFS.Notifications
{
    [Authorize]
    public class NotificationAppService : FAFSAppService, INotificationAppService
    {
        private readonly IRepository<AppNotification, Guid> _notificationRepository;

        public NotificationAppService(IRepository<AppNotification, Guid> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<PagedResultDto<AppNotificationDto>> GetListAsync(GetAppNotificationsInput input)
        {
            var query = await _notificationRepository.GetQueryableAsync();
            var currentUserId = CurrentUser.GetId();

            query = query.Where(n => n.UserId == currentUserId);

            if (input.IsRead.HasValue)
            {
                query = query.Where(n => n.IsRead == input.IsRead.Value);
            }

            var totalCount = await AsyncExecuter.CountAsync(query);
            
            var items = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(n => n.CreationTime)
                     .Skip(input.SkipCount)
                     .Take(input.MaxResultCount)
            );

            return new PagedResultDto<AppNotificationDto>(
                totalCount,
                ObjectMapper.Map<List<AppNotification>, List<AppNotificationDto>>(items)
            );
        }

        public async Task<int> GetUnreadCountAsync()
        {
            var currentUserId = CurrentUser.GetId();
            return await _notificationRepository.CountAsync(n => n.UserId == currentUserId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);
            if (notification.UserId == CurrentUser.GetId())
            {
                notification.SetAsRead();
                await _notificationRepository.UpdateAsync(notification);
            }
        }

        public async Task MarkAllAsReadAsync()
        {
            var currentUserId = CurrentUser.GetId();
            var unreadNotifications = await _notificationRepository.GetListAsync(n => n.UserId == currentUserId && !n.IsRead);

            foreach (var notification in unreadNotifications)
            {
                notification.SetAsRead();
            }

            if (unreadNotifications.Any())
            {
                await _notificationRepository.UpdateManyAsync(unreadNotifications);
            }
        }
    }
}
