using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BigGamingOrderQueryResponse
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; }
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
        [JsonPropertyName("etag")]
        public string Etag { get; set; }
        [JsonPropertyName("page")]
        public int Page { get; set; }
        [JsonPropertyName("items")]
        public BigGamingOrderQueryItemResponse[] Items { get; set; }
    }
}
