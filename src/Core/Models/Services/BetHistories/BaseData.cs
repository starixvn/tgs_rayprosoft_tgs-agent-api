using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.BetHistories.JsonConverter;

namespace TGS.Core.Models.Services.BetHistories
{
    [JsonConverter(typeof(DataConverter))]
    public abstract record BaseData
    {
        public BetHistoryDataType Type { get; init; }
    }
}