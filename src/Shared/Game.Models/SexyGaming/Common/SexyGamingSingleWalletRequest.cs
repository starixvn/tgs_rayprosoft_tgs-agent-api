using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.SexyGaming.Common
{
    public record SexyGamingSingleWalletRequest
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("extension1")]
        public string Extension1 { get; set; }
    }
}
