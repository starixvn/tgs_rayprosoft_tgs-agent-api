using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.BigGaming.Common;
using TGS.Shared.Game.Models.BigGaming.Enums;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BGOrderStatusRequest : BGSignParamsRequest
    {
        [JsonPropertyName("moduleId")]
        public BGModuleId ModuleId { get; set; }
        [JsonPropertyName("tranId")]
        public string TranId { get; set; }
        [JsonPropertyName("orderIds")]
        public long[] OrderIds { get; set; }
        [JsonPropertyName("orderMap")]
        public BGOrderStatusOrderMapRequest[] OrderMap { get; set; }
    }
}
