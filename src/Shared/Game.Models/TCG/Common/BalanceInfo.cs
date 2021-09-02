using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.TCG.Common
{
    public record BalanceInfo
    {
        [JsonPropertyName("usernam")]
        public string Usernam { get; init; }
        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }
    }
}
