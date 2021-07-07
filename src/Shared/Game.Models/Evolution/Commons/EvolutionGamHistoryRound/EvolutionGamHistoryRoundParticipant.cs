using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound
{
    public record EvolutionGamHistoryRoundParticipant
    {
        [JsonPropertyName("bets")]
        public EvolutionGamHistoryRoundParticipantBet[] Bets { get; set; }
        [JsonPropertyName("casinoId")]
        public string CasinoId { get; set; }
        [JsonPropertyName("channel")]
        public string Channel { get; set; }
        [JsonPropertyName("configOverlays")]
        public object[] ConfigOverlays { get; set; }
        [JsonPropertyName("currency")]
        public string Currency { get; set; }
        [JsonPropertyName("device")]
        public string Device { get; set; }
        [JsonPropertyName("os")]
        public string Os { get; set; }
        [JsonPropertyName("playerId")]
        public string PlayerId { get; set; }
        [JsonPropertyName("screenName")]
        public string ScreenName { get; set; }
        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; }
        [JsonPropertyName("casinoSessionId")]
        public string CasinoSessionId { get; set; }
        [JsonPropertyName("sideBetBankerPair")]
        public string SideBetBankerPair { get; set; }
        [JsonPropertyName("sideBetPlayerPair")]
        public string SideBetPlayerPair { get; set; }
        [JsonPropertyName("subType")]
        public string SubType { get; set; }
    }
}
