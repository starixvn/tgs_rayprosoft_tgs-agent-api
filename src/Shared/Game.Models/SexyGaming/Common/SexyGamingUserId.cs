using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.SexyGaming.Common
{
    public record SexyGamingUserId
    {
        [JsonPropertyName("userId")]
        public string UserId { get; init; }
    }
}
