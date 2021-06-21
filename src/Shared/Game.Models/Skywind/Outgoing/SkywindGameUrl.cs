using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Skywind.Outgoing
{
    public record SkywindGameUrl
    {
        [JsonPropertyName("ticket")]
        public string Ticket { get; set; }
    }
}
