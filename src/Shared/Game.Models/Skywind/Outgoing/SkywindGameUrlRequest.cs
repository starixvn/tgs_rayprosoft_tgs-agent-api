using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Skywind.Outgoing
{
    public class SkywindGameUrlRequest
    {
        [JsonPropertyName("playerCode")]
        public string PlayerCode { get; set; }
        [JsonPropertyName("gameCode")]
        public string GameCode { get; set; }
        [JsonPropertyName("ticket")]
        public string Ticket { get; set; }
    }
}
