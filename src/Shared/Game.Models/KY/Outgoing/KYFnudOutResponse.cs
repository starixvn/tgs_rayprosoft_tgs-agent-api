using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.KY.Commons;

namespace TGS.Shared.Game.Models.KY.Outgoing
{
    public record KYFnudOutResponse : KYBaseDataResponse
    {
        [JsonPropertyName("money")]
        public string Balance { get; init; }
    }
}
