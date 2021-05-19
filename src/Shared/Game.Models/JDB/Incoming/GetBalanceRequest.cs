using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public record GetBalanceRequest : BaseActionRequest
    {
        [JsonPropertyName("currency")]
        public string Currency { get; init; }
    }
}