using TGS.Core.Models.Attributes;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.BetHistories.JsonConverter;

namespace TGS.Core.Models.Services.BetHistories
{
    [InterfaceJsonConverter(typeof(DataConverter))]
    public interface IData
    {
        public BetHistoryDataType Type { get; }
    }
}
