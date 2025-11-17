using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Values;

namespace FAFS.Destinations
{
    public class Destination : AuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; } = string.Empty;
        public string Country { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string PhotoUrl { get; private set; } = string.Empty;
        public Coordinates Coordinates { get; private set; } = default!;
        public DateTime LastUpdated { get; private set; } = DateTime.Now;

        // EF requiere ctor protegido
        protected Destination() { }

        public Destination(
            Guid id,
            string name,
            string country,
            string city,
            string photoUrl,
            DateTime lastUpdated,
            Coordinates coordinates
        ) : base(id)
        {
            Name = name;
            Country = country;
            City = city;
            PhotoUrl = photoUrl;
            LastUpdated = lastUpdated;
            Coordinates = coordinates;
        }
    }

    // ✅ Value Object correcto
    public class Coordinates : ValueObject
    {
        public string Latitude { get; private set; } = string.Empty;
        public string Longitude { get; private set; } = string.Empty;

        // EF necesita constructor protegido
        protected Coordinates() { }

        public Coordinates(string latitude, string longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        // Esto hace que el ValueObject compare por contenido
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Latitude;
            yield return Longitude;
        }
    }
}
