using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.TCG.Common
{
    public record TCGSingleWalletRequest
    {
        [Required]
        [JsonPropertyName("method")]
        public int Method { get; init; }
        [Required]
        [JsonPropertyName("request_time")]
        public long RequestTime { get; init; }
    }
}
