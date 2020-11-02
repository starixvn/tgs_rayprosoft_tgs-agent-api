using System.Text.Json.Serialization;

namespace TGS.Core.Models.Messages.Pluggables
{
    public record GetGameUrlResponse : TgsResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
