using System;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedAnnouncement
    {
        public Guid Id { get; init; }
        public string Subject { get; init; }
        public string Content { get; init; }
        public DateTimeOffset From { get; init; }
        public DateTimeOffset To { get; init; }
    }
}