using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.WGHorse.Commons;

namespace TGS.Shared.Game.Models.WGHorse.Outgoing
{
    public record WGHorseGameRecordResponse
    {
        [JsonPropertyName("ReturnCode")]
        public string ReturnCode { get; set; }
        [JsonPropertyName("Page")]
        public string Page { get; set; }
        [JsonPropertyName("PageTotal")]
        public string PageTotal { get; set; }
        [JsonPropertyName("PageSize")]
        public string PageSize { get; set; }
        [JsonPropertyName("RaceDateFrom")]
        public string RaceDateFrom { get; set; }
        [JsonPropertyName("RaceDateTo")]
        public string RaceDateTo { get; set; }
        [JsonPropertyName("ArrayInvoice")]
        public WGHorseGameRecord[] ArrayInvoice { get; set; }
    }
}
