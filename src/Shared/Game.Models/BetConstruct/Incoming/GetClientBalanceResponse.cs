using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BetConstruct.Incoming
{
    public record GetClientBalanceResponse : ErrorResponse
    {
        [JsonPropertyName("Balance")]
        public decimal Balance { get; set; }
    }
}
