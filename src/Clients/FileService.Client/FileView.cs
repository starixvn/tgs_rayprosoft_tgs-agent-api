using System;

namespace TGS.Apis.FileService.Clients
{
    public record FileView
    {
        public string Id { get; init; }

        public string FileName { get; init; }

        public string ContentType { get; init; }

        public decimal FileLength { get; init; }

        public DateTimeOffset CreatedOn { get; init; }

        public string ExternalUrl { get; init; }
    }
}
