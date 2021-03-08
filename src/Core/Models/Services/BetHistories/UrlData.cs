using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.BetHistories
{
    public sealed record UrlData : BaseData
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }

        [JsonPropertyName("type")]
        public BetHistoryDataType Type { get; init; } = BetHistoryDataType.Url;
    }
}