// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Outgoing
{
    public record LoginRequest : BasePlayerRequest
    {
        [JsonPropertyName("Portfolio")]
        public string Portfolio { get; init; }
    }
}
