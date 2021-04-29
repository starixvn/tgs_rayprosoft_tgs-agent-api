using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Core.Models.Tokens;

namespace TGS.Services.PluggableServices.Models
{
    public record GameProviderWalletFnudRequest
    {
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }
        [JsonPropertyName("token")]
        public PlayerTokenData Token { get; init; }
        [JsonPropertyName("amount")]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Amount { get; init; }
    }
}
