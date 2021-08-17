using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.Services.Operator.GameProviders;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Game
{
    internal class GetGpCodeQueryService : IGetService<GetGpCodeQuery, BrandSubGameProviderInfo>
    {
        private readonly IGameProviderRepository _gameProviderRepository;

        public GetGpCodeQueryService(
            IGameProviderRepository gameProviderRepository)
        {
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public async ValueTask<BrandSubGameProviderInfo> GetAsync(
            GetGpCodeQuery query,
            CancellationToken cancellationToken = default)
        {
            var subGp = await _gameProviderRepository
                .QueryAllSubGameProvidersAsync(query.BrandId)
                .Where(bgp => bgp.Code == query.SubGpCode)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (subGp == null)
                throw new ArgumentException(
                    $"No game provider of code '{query.SubGpCode}' is found");

            return subGp;
        }
    }
}
