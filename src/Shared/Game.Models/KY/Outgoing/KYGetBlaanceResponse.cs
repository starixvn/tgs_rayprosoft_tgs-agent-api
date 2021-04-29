using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.KY.Outgoing
{
    public record KYGetBlaanceResponse : KYBaseDataResponse
    {
        [JsonPropertyName("money")]
        public decimal Balance { get; init; }
    }
}
