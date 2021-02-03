using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.BetHistories
{
    public sealed record JsonData : BaseData
    {
        public JsonData() : base(BetHistoryDataType.Json)
        {
            
        }
    }
}
