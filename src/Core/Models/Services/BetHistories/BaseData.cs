using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.BetHistories.JsonConverter;

namespace TGS.Core.Models.Services.BetHistories
{
    [JsonConverter(typeof(DataConverter))]
    public abstract record BaseData
    {
        protected BaseData(BetHistoryDataType type)
        {
            Type = type;
        }

        [JsonPropertyName("type")]
        public BetHistoryDataType Type { get; }
    }
}