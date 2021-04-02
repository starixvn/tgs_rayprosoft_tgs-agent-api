using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.StationLetters
{
    internal class GetStationLettersQueryService : IQueryService<GetStationLettersQuery, StationLetter>
    {
        private readonly IStationLetterRepository _stationLetterRepository;

        public GetStationLettersQueryService(IStationLetterRepository stationLetterRepository)
        {
            _stationLetterRepository = stationLetterRepository ?? throw new ArgumentNullException(nameof(stationLetterRepository));
        }

        public IAsyncEnumerable<StationLetter> QueryAsync(GetStationLettersQuery query)
        {
            if (query.Paging.ShowAll)
                return _stationLetterRepository.QueryStationLettersAsync(query.PlayerCode);

            var paging = new Paging<StationLetter>
            {
                Page = query.Paging.Page,
                RowCount = query.Paging.Rows
            };

            if (!string.IsNullOrWhiteSpace(query.Paging.SortBy))
            {
                var type = typeof(StationLetter);

                var orderByExp = type.MakeMemberAccess(query.Paging.SortBy);

                paging.OrderBy = orderByExp;
                paging.IsDesc = query.Paging.IsDesc;
            }

            return _stationLetterRepository.QueryStationLettersAsync(query.PlayerCode, paging);
        }
    }
}
