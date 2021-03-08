using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.BetHistories
{
    public sealed record JsonData : ValueData
    {
        public JsonData() : base(BetHistoryDataType.Json)
        {
        }
    }
}