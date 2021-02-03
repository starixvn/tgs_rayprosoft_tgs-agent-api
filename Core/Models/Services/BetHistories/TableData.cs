using System.Collections.Generic;
using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.BetHistories
{
    public record TableData : IData
    {
        [JsonPropertyName("type")]
        public BetHistoryDataType Type => BetHistoryDataType.Table;

        [JsonPropertyName("headers")]
        public string[] Headers { get; init; }

        [JsonPropertyName("rows")]
        public List<Dictionary<string, string>> Rows { get; init; }
    }
}
