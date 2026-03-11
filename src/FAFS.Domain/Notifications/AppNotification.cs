using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace FAFS.Notifications
{
    public class AppNotification : AuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public bool IsRead { get; internal set; }
        public string Type { get; private set; }

        protected AppNotification()
        {
        }

        public AppNotification(
            Guid id,
            Guid userId,
            string title,
            string message,
            string type,
            bool isRead = false) : base(id)
        {
            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            IsRead = isRead;
        }

        public void SetAsRead()
        {
            IsRead = true;
        }
    }
}
