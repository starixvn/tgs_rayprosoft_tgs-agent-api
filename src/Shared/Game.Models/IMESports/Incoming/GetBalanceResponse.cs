using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Incoming
{
    public record GetBalanceResponse : BaseResponse
    {
        [JsonPropertyName("PlayerID")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("Currency")]
        public string Currency { get; init; }
    }
}
