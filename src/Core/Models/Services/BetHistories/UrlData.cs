using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.BetHistories
{
    public sealed record UrlData : BaseData
    {
        public UrlData() : base(BetHistoryDataType.Url)
        {
        }

        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}