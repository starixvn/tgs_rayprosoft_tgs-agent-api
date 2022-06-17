using System;
using System.Collections.Generic;

namespace TGS.Services.Agent;

public interface IGameProviderRepository
{
    IAsyncEnumerable<GameProviderInfo> GetGameProvidersAsync(Guid brandId);
}
