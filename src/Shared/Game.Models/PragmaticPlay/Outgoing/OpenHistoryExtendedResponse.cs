using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Outgoing
{
    public record OpenHistoryExtendedResponse : ErrorResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}