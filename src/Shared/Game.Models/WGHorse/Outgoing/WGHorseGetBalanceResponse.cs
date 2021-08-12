using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.WGHorse.Commons;

namespace TGS.Shared.Game.Models.WGHorse.Outgoing
{
    public record WGHorseGetBalanceResponse : WGHorseResponse
    {
        [JsonPropertyName("Balance")]
        public decimal Balance { get; init; }
        [JsonPropertyName("Currency")]
        public string Currency { get; init; }
    }
}
