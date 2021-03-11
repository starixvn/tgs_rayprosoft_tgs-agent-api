using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.BetHistories
{
    public abstract record ValueData : BaseData
    {
        protected ValueData(BetHistoryDataType type) : base(type)
        {
        }

        [JsonPropertyName("value")]
        public string Value { get; init; }
    }
}