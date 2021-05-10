using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    public record CheckOrCreateAccountRequest : BaseRequest
    {
        [JsonPropertyName("method")]
        public string Method { get; } = "lg";
    }
}