using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BetConstruct.Incoming
{
    public class GetClientBalanceResponse
    {
        [JsonPropertyName("Balance")]
        public decimal Balance { get; set; }

        [JsonPropertyName("ErrorCode")]
        public string ErrorCode { get; set; }

        [JsonPropertyName("ErrorText")]
        public string ErrorText { get; set; }
    }
}
