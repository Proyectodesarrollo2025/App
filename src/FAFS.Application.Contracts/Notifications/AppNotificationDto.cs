using System;
using Volo.Abp.Application.Dtos;

namespace FAFS.Notifications
{
    public class AppNotificationDto : EntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string Type { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
