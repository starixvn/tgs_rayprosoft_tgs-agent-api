using System;
using System.Collections.Generic;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.StationLatters
{
    internal class ListStationLattersQueryService : IQueryService<ListStationLattersQuery, StationLatter>
    {
        private readonly IStationLatterRepository _stationLatterRepository;

        public ListStationLattersQueryService(IStationLatterRepository stationLatterRepository)
        {
            _stationLatterRepository = stationLatterRepository ?? throw new ArgumentNullException(nameof(stationLatterRepository));
        }

        public IAsyncEnumerable<StationLatter> QueryAsync(ListStationLattersQuery query)
        {
            return _stationLatterRepository.QueryStationLattersAsync(query.PlayerCode);
        }
    }
}
