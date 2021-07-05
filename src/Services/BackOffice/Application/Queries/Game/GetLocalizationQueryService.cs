using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.Game
{
    internal class GetLocalizationQueryService : IGetService<GetGameLocalizationQuery, GameLocalizationInfo>
    {
        private readonly ITgsRepository _tgsRepository;

        public GetLocalizationQueryService(
            ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<GameLocalizationInfo> GetAsync(
            GetGameLocalizationQuery query,
            CancellationToken cancellationToken = default)
        {
            var gameProvider = await _tgsRepository.GameProviders.FirstOrDefaultAsync(x => x.Code == query.GpCode);

            var gpId = gameProvider.Id;

            var providerLanguages = await _tgsRepository.GameProviderLanguages
                .AsAsyncEnumerable()
                .Where(x => x.GameProviderId == gpId)
                .ToListAsync();

            var games = await _tgsRepository.Games
                .AsAsyncEnumerable()
                .Where(x => x.GameProviderId == gpId)
                .ToListAsync();

            return new GameLocalizationInfo()
            {
                Languages = providerLanguages.Select(x => x.CultureCode).ToList(),
                Games = games.Select(x =>
                    new GameData()
                    {
                        GameId = x.Id,
                        GameName = x.Name,
                        Localization = x.Localization
                    })
                    .ToList()
            };
        }
    }
}