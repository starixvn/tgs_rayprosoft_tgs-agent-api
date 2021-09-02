using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.TCG.Common;

namespace TGS.Shared.Game.Models.TCG.Incoming
{
    public record TCGBalanceResponse : TCGSingleWalletResponse
    {
        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }
    }
}
