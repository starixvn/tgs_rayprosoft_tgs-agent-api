// unset

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.JDB.Outgoing
{
    public record BetHistoryResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        public BetHistoryPageUrl[] Data { get; init; }
    }

    public record BetHistoryPageUrl
    {
        [JsonPropertyName("path")]
        public string Path { get; init; }
    }
}