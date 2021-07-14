using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.GameResult
{
    public record LightningDice
    {
        [JsonPropertyName("first")]
        public int First { get; set; }
        [JsonPropertyName("second")]
        public int Second { get; set; }
        [JsonPropertyName("third")]
        public int Third { get; set; }
    }
}
