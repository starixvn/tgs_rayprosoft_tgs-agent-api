using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator.Games
{
    public interface IGameRepository
    {
        IAsyncEnumerable<GameTypeDataInfo> QueryGameTypeInfosAsync(
            IEnumerable<Guid> gameProviderIds,
            IEnumerable<GameStatus> statuses,
            IEnumerable<GamePlatformType> gamePlatformTypes);

        IAsyncEnumerable<GameDataInfo> QueryGamesAsync(
            IEnumerable<Guid> gameProviderIds,
            IEnumerable<GameStatus> statuses,
            IEnumerable<GamePlatformType> gamePlatformTypes);
    }
}