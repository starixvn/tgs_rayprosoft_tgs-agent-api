using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record CancelBetResponse : BaseResponse
    {
        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }
    }
}