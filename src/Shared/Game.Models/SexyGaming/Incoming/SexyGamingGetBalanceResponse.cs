using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.SexyGaming.Common;

namespace TGS.Shared.Game.Models.SexyGaming.Incoming
{
    public record SexyGamingGetBalanceResponse : SexyGamingBaseResponse
    {
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }
        [JsonPropertyName("balanceTs")]
        public DateTimeOffset BalanceTs { get; set; }
    }
}
