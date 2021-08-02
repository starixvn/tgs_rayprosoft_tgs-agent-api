using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.BigGaming.Commons;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BigGamingGPGameRecordFishingRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("gameType")]
        public int GameType => 1;
        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }
        [JsonPropertyName("endTime")]
        public DateTime EndTime { get; set; }
        [JsonPropertyName("pageIndex")]
        public string PageIndex { get; set; }
        [JsonPropertyName("pageSize")]
        public int PageSize => 500;
    }
}
