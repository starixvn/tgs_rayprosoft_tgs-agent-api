using System.Collections.Generic;
using System.Threading;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.GameProvder
{
    internal class GetGameProvidersQueryService : IQueryService<GetGameProvidersQuery, GameProviderInfo>
    {
        private readonly IGameProviderRepository _gameProviderRepository;

        public GetGameProvidersQueryService(IGameProviderRepository gameProviderRepository)
        {
            _gameProviderRepository = gameProviderRepository;
        }

        public IAsyncEnumerable<GameProviderInfo> QueryAsync(GetGameProvidersQuery query, CancellationToken cancellationToken = default)
            => _gameProviderRepository.GetGameProvidersAsync(query.BrandId);
    }
}
