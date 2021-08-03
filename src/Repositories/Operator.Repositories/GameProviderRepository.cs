using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
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

        public async ValueTask<GameProviderDataInfo?> GetGameProviderByIdAsync(Guid gameProviderId)
        {
            var gp = await _tgsRepository.GameProviders
                .FindAsync(gameProviderId)
                .ConfigureAwait(false);

            if (gp == null)
                return null;

            return new GameProviderDataInfo(
                gp.Id,
                gp.Code,
                gp.Status,
                gp.TransferWallet);
        }

        public async ValueTask<SubGameProviderWithLocationDataInfo?> GetSubGameProviderDataInfoAsync(string subGpCode)
            => await _tgsRepository.SubGameProviders
                .Include(sgp => sgp.GameProvider)
                .Where(sgp => sgp.Code == subGpCode)
                .Select(sgp => new SubGameProviderWithLocationDataInfo(
                    sgp.GameProviderId,
                    sgp.GameProvider.Code,
                    sgp.Code,
                    sgp.Name,
                    sgp.Description,
                    sgp.Localization))
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

        public IAsyncEnumerable<GameProviderCurrencyDataInfo> QueryGameProviderCurrenciesAsync(Guid gameProviderId)
        {
            var query = from gpc in _tgsRepository.GameProviderCurrencies.AsQueryable()
                        where gpc.GameProviderId == gameProviderId
                        select new GameProviderCurrencyDataInfo(
                            gpc.GameProviderId,
                            gpc.CurrencyCode);

            return query.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<GameProviderDataInfo> QueryGameProvidersAsync()
        {
            var query = from gp in _tgsRepository.GameProviders.AsQueryable()
                        select new GameProviderDataInfo(
                            gp.Id,
                            gp.Code,
                            gp.Status,
                            gp.TransferWallet);

            return query.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<SubGameProviderWithLocationDataInfo> QuerySubGameProvidersAsync()
        {
            var query = from sgp in _tgsRepository.SubGameProviders
                            .AsQueryable()
                            .Include(sgp => sgp.GameProvider)
                        select new SubGameProviderWithLocationDataInfo(
                            sgp.GameProviderId,
                            sgp.GameProvider.Code,
                            sgp.Code,
                            sgp.Name,
                            sgp.Description,
                            sgp.Localization);

            return query.ToAsyncEnumerable();
        }
    }
}
