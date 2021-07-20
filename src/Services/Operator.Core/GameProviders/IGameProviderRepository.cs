using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator.GameProviders
{
    public interface IGameProviderRepository
    {
        IAsyncEnumerable<GameProviderDataInfo> QueryGameProvidersAsync(IEnumerable<Guid> gameProviderIds,
            IEnumerable<GamePlatformType> gamePlatformTypes);

        IAsyncEnumerable<SubGameProviderInfo> QuerySubGameProvidersAsync(IEnumerable<Guid> gameProviderIds);
    }
}