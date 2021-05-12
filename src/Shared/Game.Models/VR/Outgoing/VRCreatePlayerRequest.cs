using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRCreatePlayerRequest
    {
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; init; }
    }
}
