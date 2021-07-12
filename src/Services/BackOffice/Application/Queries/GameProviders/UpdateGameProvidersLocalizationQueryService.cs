using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.GameProviders
{
    internal class
        UpdateGameProvidersLocalizationQueryService : IGetService<UpdateGameProvidersLocalizationQuery,
            UpdateGameProvidersLocationInfo>
    {
        private readonly ITgsRepository _tgsRepository;

        public UpdateGameProvidersLocalizationQueryService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<UpdateGameProvidersLocationInfo>
            GetAsync(UpdateGameProvidersLocalizationQuery query,
                CancellationToken cancellationToken = default)
        {
            var queryGameProviderIds = query.GameProviders.Select(x => x.Id).ToList();

            var existGameProviderIds = await _tgsRepository.GameProviders.AsAsyncEnumerable()
                .Where(p => queryGameProviderIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToArrayAsync(cancellationToken);

            var diffGameProviders =
                query.GameProviders.Select(x => x.Id).ToArray().Except(existGameProviderIds).ToArray();

            if (diffGameProviders.Length == 0)
            {
                foreach (var item in existGameProviderIds)
                {
                    var model = query.GameProviders.First(x => x.Id == item);
                    var game = await _tgsRepository.GameProviders.FirstOrDefaultAsync(x => x.Id == item,
                        cancellationToken);
                    game.Localization = model.Localization.ToJson();
                    game.UpdatedByUsername = query.EditorUsername;
                    game.UpdatedOn = DateTimeOffset.Now;
                }

                await _tgsRepository.SaveChangesAsync(cancellationToken);

                return new UpdateGameProvidersLocationInfo {DiffGameProviderIds = Array.Empty<Guid>()};
            }

            return new UpdateGameProvidersLocationInfo {DiffGameProviderIds = diffGameProviders};
        }
    }
}