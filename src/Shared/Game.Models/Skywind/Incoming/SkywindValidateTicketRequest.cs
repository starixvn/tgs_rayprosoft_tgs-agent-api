using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.Skywind.Common;

namespace TGS.Shared.Game.Models.Skywind.Incoming
{
    public record SkywindValidateTicket : SkywindSingleWalletRequest
    {
        [JsonPropertyName("ticket")]
        public string Ticket { get; set; }
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
    }
}
