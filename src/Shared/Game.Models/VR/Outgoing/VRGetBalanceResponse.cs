using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRGetBalanceResponse
    {
        [JsonPropertyName("playerName")]
        public string PlayerName { get; set; }
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }
        [JsonPropertyName("games")]
        public IEnumerable<VRGetBalanceGamesResponse> Games { get; set; }
    }
}
