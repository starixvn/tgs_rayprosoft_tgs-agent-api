using System.Collections.Generic;

namespace TGS.Services.Operator
{
    public interface IStationLatterRepository
    {
        IAsyncEnumerable<StationLatter> QueryStationLattersAsync(string playerCode);
    }
}
