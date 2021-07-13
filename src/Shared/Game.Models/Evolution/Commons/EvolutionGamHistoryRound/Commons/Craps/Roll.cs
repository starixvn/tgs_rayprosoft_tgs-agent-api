using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.Craps
{
    public record Roll
    {
        [JsonPropertyName("rollId")]
        public string RollId { get; set; }
        [JsonPropertyName("startedAt")]
        public DateTimeOffset StartedAt { get; set; }
        [JsonPropertyName("result")]
        public Result Result { get; set; }
    }
}
