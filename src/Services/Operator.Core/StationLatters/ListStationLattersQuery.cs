using TGS.Core.Models.Messages;

namespace TGS.Services.Operator.StationLatters
{
    public record ListStationLattersQuery(string PlayerCode, PageableRequest Paging);
}
