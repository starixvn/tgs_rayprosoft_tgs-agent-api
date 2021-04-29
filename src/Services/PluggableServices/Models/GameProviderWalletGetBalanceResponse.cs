using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Services.PluggableServices.Models
{
    public record GameProviderWalletGetBalanceResponse
    {
        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }
    }
}
