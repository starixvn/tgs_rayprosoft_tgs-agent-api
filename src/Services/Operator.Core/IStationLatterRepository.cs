using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IStationLatterRepository
    {
        ValueTask<int> QueryStationLattersTotalCountAsync(string playerCode);

        IAsyncEnumerable<StationLatter> QueryStationLattersAsync(string playerCode, Paging<StationLatter> paging);

        IAsyncEnumerable<StationLatter> QueryStationLattersAsync(string playerCode);
    }
}
