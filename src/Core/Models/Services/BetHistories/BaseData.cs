using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.BetHistories
{
    public abstract record BaseData : IData
    {
        protected BaseData(BetHistoryDataType type)
        {
            Type = type;
        }

        [JsonPropertyName("value")]
        public string Value { get; init; }

        [JsonPropertyName("type")]
        public BetHistoryDataType Type { get; init; }
    }
}