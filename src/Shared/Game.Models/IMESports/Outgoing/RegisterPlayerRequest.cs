// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Outgoing
{
    public record RegisterPlayerRequest : BasePlayerRequest
    {
        [JsonPropertyName("Currency")]
        public string Currency { get; init; }

        [JsonPropertyName("Password")] public string Password => "Password";
    }
}