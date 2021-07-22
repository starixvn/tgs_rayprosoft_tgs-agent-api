using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator.GameProviders
{
    public interface IGameProviderRepository
    {
        IAsyncEnumerable<GameProviderDataInfo> QueryGameProvidersAsync(IEnumerable<Guid> gameProviderIds,
            IEnumerable<Status> statuses);

        IAsyncEnumerable<SubGameProviderWithLocationDataInfo> QuerySubGameProvidersAsync(
            IEnumerable<Guid> gameProviderIds);

        IAsyncEnumerable<SubGameProviderDataInfo> QuerySubGameProvidersAsync(
            string? subGameProviderCode = null);

        IAsyncEnumerable<GameProviderCurrencyDataInfo> QueryGameProviderCurrenciesAsync(
            IEnumerable<Guid> gameProviderIds);
    }
}