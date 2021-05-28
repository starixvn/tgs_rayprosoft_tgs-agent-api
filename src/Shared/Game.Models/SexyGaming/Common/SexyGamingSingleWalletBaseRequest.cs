using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.SexyGaming.Common
{
    public record SexyGamingSingleWalletBaseRequest
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
