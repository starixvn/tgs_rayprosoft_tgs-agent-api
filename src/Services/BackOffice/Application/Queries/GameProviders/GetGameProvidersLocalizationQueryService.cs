using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.GameProviders
{
    internal class GetGameProvidersLocalizationQueryService : IGetService<GetGameProvidersLocalizationQuery, GameProvidersLocalizationInfo>
    {
        private readonly ITgsRepository _tgsRepository;

        public GetGameProvidersLocalizationQueryService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<GameProvidersLocalizationInfo> GetAsync(
            GetGameProvidersLocalizationQuery query,
            CancellationToken cancellationToken = default)
        {
            var languages = await _tgsRepository.GameProviderLanguages
                .AsAsyncEnumerable()
                .GroupBy(x => x.CultureCode)
                .Select(x => x.Key)
                .ToArrayAsync();
            var gameProviders = await _tgsRepository.GameProviders.ToArrayAsync();

            return new GameProvidersLocalizationInfo()
            {
                Languages = languages,
                GameProviders = gameProviders.Select(x =>
                    new GameProviderData()
                    {
                        GameProviderId = x.Id,
                        GameProviderName = x.Name,
                        Localization = x.Localization
                    })
            };
        }
    }
}