using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound
{
    public record EvolutionGamHistoryRoundParticipantBet
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("payout")]
        public decimal Payout { get; set; }
        [JsonPropertyName("placedOn")]
        public DateTime PlacedOn { get; set; }
        [JsonPropertyName("stake")]
        public decimal Stake { get; set; }
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; }
        [JsonPropertyName("owTransactionId")]
        public string OwTransactionId { get; set; }
    }
}
