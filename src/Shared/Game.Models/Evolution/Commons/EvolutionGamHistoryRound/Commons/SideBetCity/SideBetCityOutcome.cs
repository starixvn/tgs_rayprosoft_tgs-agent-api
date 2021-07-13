using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.SideBetCity
{
    public record SideBetCityOutcome
    {
        [JsonPropertyName("threeCards")]
        public SideBetCityCards ThreeCards { get; set; }
        [JsonPropertyName("fiveCards")]
        public SideBetCityCards FiveCards { get; set; }
        [JsonPropertyName("sevenCards")]
        public SideBetCityCards SevenCards { get; set; }
    }
}
