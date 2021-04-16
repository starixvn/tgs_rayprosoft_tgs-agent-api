using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record JackpotWinRequest : TransactionRequest
    {
        [JsonPropertyName("jackpotId")]
        public string JackpotId { get; init; }
    }
}