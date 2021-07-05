using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons
{
    public record InstantRouletteOutcome
    {
        [JsonPropertyName("number")]
        public string Number { get; set; }
        [JsonPropertyName("number")]
        public string Wheel { get; set; }
        [JsonPropertyName("spin")]
        public string Spin { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("color")]
        public string Color { get; set; }
    }
}
