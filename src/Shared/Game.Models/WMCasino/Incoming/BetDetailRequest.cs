using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Incoming
{
    public record BetDetailRequest : BaseRequest
    {
        [JsonPropertyName("totalorder")]
        public int TotalCount { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("result")]
        public BetResult[] Results { get; set; }
    }
}