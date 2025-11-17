using System;
using Volo.Abp.Domain.Entities;
using FAFS;

public class DestinationRating : AggregateRoot<Guid>, IUserOwned
{
    public Guid UserId { get; set; }
    public Guid DestinationId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.UtcNow;

    protected DestinationRating() { }

    public DestinationRating(Guid id, Guid userId, Guid destinationId, int score, string? comment = null)
        : base(id)
    {
        UserId = userId;
        DestinationId = destinationId;
        Score = score;
        Comment = comment;
    }
}
