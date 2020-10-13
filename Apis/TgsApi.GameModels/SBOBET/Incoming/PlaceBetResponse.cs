using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Incoming
{
    public record PlaceBetResponse : BaseResponse
    {
        [JsonPropertyName("betAmount")]
        public decimal BetAmount { get; init; }
    }
}
