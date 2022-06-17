using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations;

public interface IGameAccessor
{
    ValueTask<IEnumerable<GameInfo>> GetBrandAvailableGamesAsync(
        Guid brandId,
        Guid gameProviderId,
        IEnumerable<string> gameExternalIds,
        IEnumerable<GamePlatformType> gamePlatformTypes);
}
