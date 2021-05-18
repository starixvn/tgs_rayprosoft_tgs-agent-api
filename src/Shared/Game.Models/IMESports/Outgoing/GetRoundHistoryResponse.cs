// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Outgoing
{
    public record GetRoundHistoryResponse : BaseResponse
    {
        [JsonPropertyName("Result")]
        public HistoryResult[] Results { get; init; }
        
        [JsonPropertyName("Pagination")]
        public Pagination Pagination { get; init; }
    }

    public record Pagination
    {
        [JsonPropertyName("CurrentPage")]
        public int CurrentPage { get; init; }
        
        [JsonPropertyName("TotalPage")]
        public int TotalPage { get; init; }
    }
}