using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.WGSports.Outgoing
{
    public record WGSportsAccessMoneyResponse
    {
        [JsonPropertyName("ReturnCode")]
        public string ReturnCode { get; init; }
    }
}
