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
        public long Timestamp { get; init; }

        [JsonPropertyName("parent")]
        public string Parent { get; init; }

        [JsonPropertyName("uid")]
        public string UserId { get; init; }

        [JsonPropertyName("lang")]
        public string Language { get; init; }

        [JsonPropertyName("gType")]
        public int GameType { get; init; }

        [JsonPropertyName("seqNo")]
        public long SequenceNo { get; init; }
    }
}