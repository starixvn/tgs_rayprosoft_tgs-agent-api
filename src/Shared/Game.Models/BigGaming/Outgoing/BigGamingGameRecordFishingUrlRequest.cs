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
        /// The order identifier
        /// </summary>
        [JsonPropertyName("orderId")]
        public long ExternalRoundId { get; set; }
        /// <summary>
        /// The game identifier
        /// </summary>
        public int GameId { get; set; }
    }
}
