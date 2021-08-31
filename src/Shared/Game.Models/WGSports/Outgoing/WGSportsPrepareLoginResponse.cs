using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.WGSports.Commons;

namespace TGS.Shared.Game.Models.WGSports.Outgoing
{
    public record WGSportsPrepareLoginResponse : WGSportsResponse
    {
        [JsonPropertyName("Token")]
        public string Token { get; init; }
    }
}
