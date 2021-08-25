using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.WGHorse.Commons;

namespace TGS.Shared.Game.Models.WGHorse.Outgoing
{
    public record WGHorsePrepareLoginResponse : WGHorseResponse
    {
        [JsonPropertyName("Token")]
        public string Token { get; init; }
    }
}
