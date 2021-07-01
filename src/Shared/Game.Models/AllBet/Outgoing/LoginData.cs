using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.AllBet.Outgoing
{
    public record LoginData
    {
        [JsonPropertyName("gameLoginUrl")]
        public string Url { get; init; }
    }
}