using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound
{
    public record EvolutionGamHistoryRoundData
    {
        [JsonPropertyName("dealer")]
        public EvolutionGamHistoryRoundDataDealer Dealer { get; set; }
        [JsonPropertyName("gameSubType")]
        public string GameSubType { get; set; }
        [JsonPropertyName("gameType")]
        public string GameType { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("gameProvider")]
        public string GameProvider { get; set; }
        [JsonPropertyName("participants")]
        public EvolutionGamHistoryRoundParticipant[] Participants { get; set; }
        [JsonPropertyName("result")]
        public JsonDocument Result { get; set; }
        [JsonPropertyName("settledAt")]
        public DateTime SettledAt { get; set; }
        [JsonPropertyName("startedAt")]
        public DateTime StartedAt { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("table")]
        public EvolutionGamHistoryRoundDataTable Table { get; set; }
    }
}
