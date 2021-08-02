using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BigGaming.Commons
{
    public record BigGamingResultGameRecordResponse<TData>
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; }
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
        [JsonPropertyName("etag")]
        public object Etag { get; set; }
        [JsonPropertyName("page")]
        public int Page { get; set; }
        [JsonPropertyName("items")]
        public TData[] Items { get; set; }
    }
}
