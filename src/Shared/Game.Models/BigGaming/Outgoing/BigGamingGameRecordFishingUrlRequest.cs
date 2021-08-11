using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.BigGaming.Commons;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BigGamingGameRecordFishingUrlRequest : BigGamingParamsSignRequest
    {
        /// <summary>
        /// The external round identifier
        /// </summary>
        [JsonPropertyName("orderId")]
        public long ExternalBetId { get; set; }
        /// <summary>
        /// The game identifier
        /// </summary>
        [JsonPropertyName("gameType")]
        public int GameId { get; set; }
        /// <summary>
        /// The locale
        /// </summary>
        [JsonPropertyName("locale")]
        public string Locale;
    }
}
