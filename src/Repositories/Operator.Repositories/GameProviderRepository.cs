using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using TGS.Services.Operator;
using TGS.Services.Operator.GameProviders;

namespace TGS.Operator.Repositories
{
    internal class GameProviderRepository : IGameProviderRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public GameProviderRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository;
        }

        public IAsyncEnumerable<SubGameProviderWithLocationDataInfo> QuerySubGameProvidersAsync(
            IEnumerable<Guid> gameProviderIds)
        {
            var query = _tgsRepository.SubGameProviders.AsQueryable()
                .Where(sgp => gameProviderIds.Contains(sgp.GameProviderId))
                .Select(sgp =>
                    new SubGameProviderWithLocationDataInfo(
                        sgp.GameProviderId,
                        sgp.Code,
                        sgp.Name,
                        sgp.Description,
                        sgp.Localization));

            return query.AsAsyncEnumerable();
        }

        public IAsyncEnumerable<GameProviderDataInfo> QueryGameProvidersAsync(IEnumerable<Guid> gameProviderIds,
            IEnumerable<Status> statuses)
        {
            var query = from gp in _tgsRepository.GameProviders.AsQueryable()
                        where gameProviderIds.Contains(gp.Id) &&
                            statuses.Contains(gp.Status)
                        select new GameProviderDataInfo(
                            gp.Id,
                            gp.Code,
                            gp.Status,
                            gp.TransferWallet);

            return query.AsAsyncEnumerable();
        }

        public IAsyncEnumerable<GameProviderCurrencyDataInfo> QueryGameProviderCurrenciesAsync(
            IEnumerable<Guid> gameProviderIds)
        {
            var query = _tgsRepository.GameProviderCurrencies.AsQueryable()
                .Where(c => gameProviderIds.Contains(c.GameProviderId))
                .Select(c => new GameProviderCurrencyDataInfo(c.GameProviderId, c.CurrencyCode));

            return query.AsAsyncEnumerable();
        }

        public IAsyncEnumerable<SubGameProviderDataInfo> QuerySubGameProvidersAsync(string? subGameProviderCode = null)
        {
            var query = _tgsRepository.SubGameProviders.AsQueryable();

            if (!string.IsNullOrEmpty(subGameProviderCode))
            {
                query = query.Where(sgp => sgp.Code == subGameProviderCode);
            }

            var info = query.Select(sgp => new SubGameProviderDataInfo(sgp.GameProviderId, sgp.Code));

            return info.AsAsyncEnumerable();
        }
    }
}