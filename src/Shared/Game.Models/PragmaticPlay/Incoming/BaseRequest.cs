using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.PragmaticPlay.Common;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record BaseRequest : HashRequest
    {
        [JsonPropertyName("providerId")]
        public string ProviderId { get; init; }

        [JsonPropertyName("bypassHashVerify")]
        public bool BypassHashVerify { get; init; }
    }
}