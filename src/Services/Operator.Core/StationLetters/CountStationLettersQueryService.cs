using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.StationLetters
{
    internal class CountStationLettersQueryService : IGetService<GetStationLettersQuery, int>
    {
        private readonly IStationLetterRepository _stationLetterRepository;

        public CountStationLettersQueryService(IStationLetterRepository stationLetterRepository)
        {
            _stationLetterRepository = stationLetterRepository ?? throw new ArgumentNullException(nameof(stationLetterRepository));
        }

        public async ValueTask<int> GetAsync(GetStationLettersQuery query, CancellationToken cancellationToken = default) =>
            await _stationLetterRepository
                .QueryStationLettersTotalCountAsync(query.PlayerCode)
                .ConfigureAwait(false);
    }
}
