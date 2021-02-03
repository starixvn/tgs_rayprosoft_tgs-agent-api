using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Services.PluggableServices.Models
{
    public record GetGameUrlResponse : TgsResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
