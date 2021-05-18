using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Incoming
{
    public record BetResponse : BaseResponse
    {
        [JsonPropertyName("Results")]
        public TransactionResponse[] Results { get; init; }
    }
}
