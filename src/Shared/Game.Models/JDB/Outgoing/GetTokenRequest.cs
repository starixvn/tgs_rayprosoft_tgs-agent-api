// unset

using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.JDB.Enums;

namespace TGS.Shared.Game.Models.JDB.Outgoing
{
    public class GetTokenRequest
    {
        [JsonPropertyName("action")]
        public int Action => (int)ActionType.GetToken;
        [JsonPropertyName("ts")]
        public long Timestamp { get; set; }
        [JsonPropertyName("parent")]
        public string Parent { get; set; }
        [JsonPropertyName("uid")]
        public string UserId { get; set; }
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }

        [JsonPropertyName("lang")]
        public string Language { get; set; }

        [JsonPropertyName("gType")]
        public string GameType { get; set; }
        [JsonPropertyName("mType")]
        public string MachineType { get; set; }
        [JsonPropertyName("windowMode")]
        public int WindowMode => 2;
        [JsonPropertyName("isAPP")]
        public bool IsApp => false;
        [JsonPropertyName("lobbyURL")]
        public string LobbyUrl => string.Empty;
    }
}