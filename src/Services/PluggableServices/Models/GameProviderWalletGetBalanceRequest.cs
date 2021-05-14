using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Core.Models.Tokens;

namespace TGS.Services.PluggableServices.Models
{
    public record GameProviderWalletGetBalanceRequest
    {
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }
        [JsonPropertyName("playercode")]
        public string PlayerCode { get; init; }
    }
}
