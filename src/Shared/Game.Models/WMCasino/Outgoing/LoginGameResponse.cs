using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Outgoing
{
    public record LoginGameResponse : ErrorResponse
    {
        [JsonPropertyName("result")]
        public string Url { get; set; }
    }
}