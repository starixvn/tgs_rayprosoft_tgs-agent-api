using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BGOrderStatusResultResponse
    {
        [JsonPropertyName("orderId")]
        public long OrderId { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
