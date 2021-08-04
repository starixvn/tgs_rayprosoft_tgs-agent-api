using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator.GameProviders
{
    public interface IGameProviderRepository
    {
        ValueTask<GameProviderDataInfo?> GetGameProviderByIdAsync(Guid gameProviderId);

        ValueTask<SubGameProviderWithLocationDataInfo?> GetSubGameProviderDataInfoAsync(string subGpCode);

        IAsyncEnumerable<BrandGameInfo> QueryAllGamesAsync(Guid brandId);

        IAsyncEnumerable<GameDataInfo> QueryAllGamesAsync(
            Guid brandId,
            IEnumerable<GameStatus> statuses);

        IAsyncEnumerable<BrandSubGameProviderInfo> QueryAllSubGameProvidersAsync(Guid brandId);

        IAsyncEnumerable<GameProviderCurrencyDataInfo> QueryGameProviderCurrenciesAsync(Guid gameProviderId);

        IAsyncEnumerable<GameProviderDataInfo> QueryGameProvidersAsync();

        IAsyncEnumerable<GameDataInfo> QueryGamesAsync(
            IEnumerable<Guid> gameProviderIds,
            IEnumerable<GameStatus> statuses,
            IEnumerable<GamePlatformType> gamePlatformTypes);

        IAsyncEnumerable<GameTypeDataInfo> QueryGameTypeInfosAsync(
            IEnumerable<Guid> gameProviderIds,
            IEnumerable<GameStatus> statuses,
            IEnumerable<GamePlatformType> gamePlatformTypes);

        IAsyncEnumerable<SubGameProviderWithLocationDataInfo> QuerySubGameProvidersAsync();
    }
}
