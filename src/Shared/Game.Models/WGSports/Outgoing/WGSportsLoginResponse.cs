using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.WGSports.Commons;

namespace TGS.Shared.Game.Models.WGSports.Outgoing
{
    public record WGSportsLoginResponse : WGSportsResponse
    {
        [JsonPropertyName("LoginUrl")]
        public string LoginUrl { get; set; }
        [JsonPropertyName("MemberId")]
        public string MemberId { get; set; }
    }
}
