using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.BetHistories
{
    public sealed record XmlData : ValueData
    {
        public XmlData() : base(BetHistoryDataType.Xml)
        {
        }
    }
}