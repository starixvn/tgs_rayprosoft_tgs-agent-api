using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Agent;

namespace TGS.Agent.Repositories;

internal class GameProviderRepository(ITgsRepository tgsRepository) : IGameProviderRepository
{
    public IAsyncEnumerable<GameProviderInfo> GetGameProvidersAsync(Guid brandId)
    {
        var gameProviders = tgsRepository.GameProviders.AsQueryable()
            .Where(gp => gp.Brands.Select(b => b.BrandId).Contains(brandId))
            .Select(gp => new GameProviderInfo
            {
                Id = gp.Id,
                Name = gp.Name,
            });

        return gameProviders.ToAsyncEnumerable();
    }
}
