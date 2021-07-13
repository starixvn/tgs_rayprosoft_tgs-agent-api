using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.Craps
{
    public record Result
    {
        [JsonPropertyName("first")]
        public int First { get; set; }
        [JsonPropertyName("second")]
        public int Second { get; set; }
    }
}
