using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.WGHorse.Commons;

namespace TGS.Shared.Game.Models.WGHorse.Outgoing
{
    public record WGHorseLoginResponse : WGHorseResponse
    {
        [JsonPropertyName("LoginUrl")]
        public string LoginUrl { get; set; }
        [JsonPropertyName("MemberId")]
        public string MemberId { get; set; }
    }
}
