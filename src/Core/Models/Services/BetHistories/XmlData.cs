using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.BetHistories
{
    public sealed record XmlData : BaseData
    {
        public XmlData() : base(BetHistoryDataType.Xml)
        {
            
        }
    }
}
