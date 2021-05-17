using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BGBalanceResponse
    {
        [JsonPropertyName("result")]
        public decimal Balance { get; set; }
    }
}
