using System.Collections.Generic;
using System.Threading;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.GameProvder;

internal class GetGameProvidersQueryService(IGameProviderRepository gameProviderRepository)
    : IQueryService<GetGameProvidersQuery, GameProviderInfo>
{
    public IAsyncEnumerable<GameProviderInfo> QueryAsync(GetGameProvidersQuery query, CancellationToken cancellationToken = default)
        => gameProviderRepository.GetGameProvidersAsync(query.BrandId);
}
