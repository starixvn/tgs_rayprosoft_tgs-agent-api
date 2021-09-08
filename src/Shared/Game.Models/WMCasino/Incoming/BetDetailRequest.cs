using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.WMCasino.Common;

namespace TGS.Shared.Game.Models.WMCasino.Incoming
{
    public record BetDetailRequest : BaseRequest
    {
        [JsonPropertyName("totalorder")]
        public int TotalCount { get; init; }

        [JsonPropertyName("page")]
        public int Page { get; init; }

        [JsonPropertyName("result")]
        public BetResult[] Results { get; init; }
    }
}