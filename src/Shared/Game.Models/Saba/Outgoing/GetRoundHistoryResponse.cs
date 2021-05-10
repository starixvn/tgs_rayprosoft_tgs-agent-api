using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Saba.Enums;

namespace TGS.Shared.Game.Models.Saba.Outgoing
{
    public record GetRoundHistoryResponse
    {
        [JsonPropertyName("error_code")]
        public GetRoundHistoryApiError ErrorCode { get; init; }

        [JsonPropertyName("message")]
        public string ErrorMessage { get; init; }

        [JsonPropertyName("Data")]
        public BetHistoryData Data { get; init; }
    }
}