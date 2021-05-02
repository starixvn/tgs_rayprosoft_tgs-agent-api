// unset

using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.JDB.Enums;

namespace TGS.Shared.Game.Models.JDB.Outgoing
{
    public class BetHistoryRequest
    {
        [JsonPropertyName("action")]
        public int Action => (int)ActionType.BetHistory;
        [JsonPropertyName("ts")]
        public long Timestamp { get; set; }
        [JsonPropertyName("parent")]
        public string Parent { get; set; }
        [JsonPropertyName("uid")]
        public string UserId { get; set; }

        [JsonPropertyName("lang")]
        public string Language { get; set; }
        
        [JsonPropertyName("gType")]
        public int GameType { get; set; }
        [JsonPropertyName("seqNo")]
        public long SequenceNo { get; set; }
    }
}