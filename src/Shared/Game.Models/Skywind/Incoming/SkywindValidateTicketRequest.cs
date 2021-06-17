using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Skywind.Common;

namespace TGS.Shared.Game.Models.Skywind.Incoming
{
    public record SkywindValidateTicketRequest : SkywindSingleWalletRequest
    {
        [JsonPropertyName("ticket")]
        public string Ticket { get; set; }
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
    }
}
