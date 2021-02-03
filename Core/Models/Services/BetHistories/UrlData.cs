using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.BetHistories
{
    public sealed record UrlData : IData
    {
        [JsonPropertyName("type")]
        public BetHistoryDataType Type => BetHistoryDataType.Url;

        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
