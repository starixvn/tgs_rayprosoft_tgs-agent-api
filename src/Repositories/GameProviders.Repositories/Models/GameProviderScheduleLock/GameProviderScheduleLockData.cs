using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.Repositories.Models.GameProviderScheduleLock
{
    public record GameProviderScheduleLockData
    {
        [JsonPropertyName("gameProviderId")]
        public Guid GameProviderId { get; init; }
        [JsonPropertyName("gameProviderCode")]
        public string GameProviderCode { get; init; }
        [JsonPropertyName("gameProviderScheduleId")]
        public Guid GameProviderScheduleId { get; init; }
        [JsonPropertyName("lockDate")]
        public DateTimeOffset LockDate { get; init; }
    }
}
