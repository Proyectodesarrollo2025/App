using System;

namespace FAFS
{
    public interface IUserOwned
    {
        Guid UserId { get; set; }
    }
}
