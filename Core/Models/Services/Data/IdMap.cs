using System;

namespace TGS.Core.Models.Services.Data
{
    public record IdMap
    {
        public Guid Id { get; init;}
        public string ExternalId { get; init; }
    }
}
