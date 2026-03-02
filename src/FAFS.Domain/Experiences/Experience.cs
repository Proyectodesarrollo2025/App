using System;
using Volo.Abp.Domain.Entities.Auditing;
using FAFS.Experiences;

namespace FAFS.Experiences
{
    public class Experience : AuditedAggregateRoot<Guid>
    {
        public Guid DestinationId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public ExperienceRating Rating { get; private set; }

        protected Experience()
        {
        }

        public Experience(
            Guid id,
            Guid destinationId,
            string title,
            string description,
            ExperienceRating rating
        ) : base(id)
        {
            DestinationId = destinationId;
            SetTitle(title);
            SetDescription(description);
            Rating = rating;
        }

        public void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be empty.");
            }
            if (title.Length > ExperienceConsts.MaxTitleLength)
            {
                throw new ArgumentException($"Title cannot exceed {ExperienceConsts.MaxTitleLength} characters.");
            }
            Title = title;
        }

        public void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be empty.");
            }
            if (description.Length > ExperienceConsts.MaxDescriptionLength)
            {
                throw new ArgumentException($"Description cannot exceed {ExperienceConsts.MaxDescriptionLength} characters.");
            }
            Description = description;
        }

        public void SetRating(ExperienceRating rating)
        {
            Rating = rating;
        }
    }
}
