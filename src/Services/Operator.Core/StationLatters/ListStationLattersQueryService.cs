using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            if (query.Paging.ShowAll)
                return _stationLatterRepository.QueryStationLattersAsync(query.PlayerCode);

            var paging = new Paging<StationLatter>
            {
                Page = query.Paging.Page,
                RowCount = query.Paging.Rows
            };

            if (!string.IsNullOrWhiteSpace(query.Paging.SortBy))
            {
                var type = typeof(StationLatter);

                var orderByExp = type.MakeMemberAccess(query.Paging.SortBy);

                paging.OrderBy = orderByExp;
                paging.IsDesc = query.Paging.IsDesc;
            }

            return _stationLatterRepository.QueryStationLattersAsync(query.PlayerCode, paging);
        }
    }
}
