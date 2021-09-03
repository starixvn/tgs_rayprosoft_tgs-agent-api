using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.TCG.Common
{
    public record TCGSEASeries
    {
        [JsonPropertyName("game_group_code")]
        public string GameGroupCode { get; init; }
        [JsonPropertyName("prize_mode_id")]
        public int PrizeModeId { get; init; }
    }
}
