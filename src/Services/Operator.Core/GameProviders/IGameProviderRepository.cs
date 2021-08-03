using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Services.Operator.GameProviders
{
    public interface IGameProviderRepository
    {
        ValueTask<GameProviderDataInfo?> GetGameProviderByIdAsync(Guid gameProviderId);

        ValueTask<SubGameProviderWithLocationDataInfo?> GetSubGameProviderDataInfoAsync(string subGpCode);

        IAsyncEnumerable<GameProviderCurrencyDataInfo> QueryGameProviderCurrenciesAsync(Guid gameProviderId);

        IAsyncEnumerable<GameProviderDataInfo> QueryGameProvidersAsync();

        IAsyncEnumerable<SubGameProviderWithLocationDataInfo> QuerySubGameProvidersAsync();
    }
}