using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Outgoing
{
    public record MemberRegisterResponse : ErrorResponse
    {
        [JsonPropertyName("result")]
        public string Result { get; init; }
    }
}