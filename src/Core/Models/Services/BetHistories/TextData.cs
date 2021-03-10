using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.BetHistories
{
    public sealed record TextData : ValueData
    {
        public TextData() : base(BetHistoryDataType.Text)
        {
        }
    }
}