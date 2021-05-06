using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.KY.Incoming
{
    public record KYGPGameRecordRequest
    {
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; init; }
        [JsonPropertyName("startTime")]
        public long StartTime { get; init; }
        [JsonPropertyName("endTime")]
        public long EndTime { get; init; }
    }
}
