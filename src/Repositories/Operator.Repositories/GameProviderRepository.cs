using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Services.Operator;
using TGS.Services.Operator.GameProviders;

namespace TGS.Operator.Repositories
{
    internal class GameProviderRepository : IGameProviderRepository
    {
        private readonly ITgsRepository _tgsRepository;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public GameProviderRepository(
            ITgsRepository tgsRepository,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
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

        public IAsyncEnumerable<BrandSubGameProviderInfo> QueryAllSubGameProvidersAsync(Guid brandId)
        {
            var query = from brandGp in _tgsRepository.BrandGameProviders.AsQueryable()
                        from gp in _tgsRepository.GameProviders
                        from subGp in _tgsRepository.SubGameProviders
                        where brandGp.GameProviderId == gp.Id
                            && brandGp.GameProviderId == subGp.GameProviderId
                            && brandGp.BrandId == brandId
                        select new BrandSubGameProviderInfo(
                            subGp.Code,
                            subGp.Name,
                            subGp.Description,
                            gp.Id,
                            gp.Code,
                            gp.TransferWallet,
                            gp.Status);

            return query.ToAsyncEnumerable();
        }

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

        public IAsyncEnumerable<GameDataInfo> QueryAllGamesAsync(Guid brandId, IEnumerable<GameStatus> statuses)
        {
            var statusesFilter = statuses.ToArray();

            var query = from bg in _tgsRepository.BrandGames.AsQueryable()
                        from game in _tgsRepository.Games
                        where bg.BrandId == brandId
                            && bg.GameId == game.Id
                            && statusesFilter.Contains(game.Status)
                            && statusesFilter.Contains(bg.Status)
                        select new GameDataInfo(
                            game.Id,
                            game.Code,
                            game.Name,
                            game.Description,
                            game.SubGameProviderCode,
                            game.Status,
                            game.Type,
                            game.PlatformType,
                            game.AvailableToBrand,
                            game.AvailableCurrencies,
                            game.Localization);

            return query.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<GameDataInfo> QueryGamesAsync(
            IEnumerable<Guid> gameProviderIds,
            IEnumerable<GameStatus> statuses,
            IEnumerable<GamePlatformType> gamePlatformTypes)
        {
            var query = _tgsRepository.Games.AsQueryable()
                .Where(g => gameProviderIds.Contains(g.GameProviderId) &&
                    statuses.Contains(g.Status) &&
                    gamePlatformTypes.Contains(g.PlatformType))
                .Select(g =>
                    new GameDataInfo(
                        g.Id,
                        g.Code,
                        g.Name,
                        g.Description,
                        g.SubGameProviderCode,
                        g.Status,
                        g.Type,
                        g.PlatformType,
                        g.AvailableToBrand,
                        g.AvailableCurrencies,
                        g.Localization));

            return query.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<GameTypeDataInfo> QueryGameTypeInfosAsync(
            IEnumerable<Guid> gameProviderIds,
            IEnumerable<GameStatus> statuses,
            IEnumerable<GamePlatformType> gamePlatformTypes)
        {
            var query = _tgsRepository.Games.AsQueryable()
                .Where(g => gameProviderIds.Contains(g.GameProviderId) &&
                    statuses.Contains(g.Status) &&
                    gamePlatformTypes.Contains(g.PlatformType))
                .Select(g => new GameTypeDataInfo(
                    g.GameProviderId,
                    g.Type,
                    g.SubGameProviderCode))
                .Distinct();

            return query.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<BrandGameInfo> QueryAllGamesAsync(Guid brandId)
        {
            var status = new[] { GameStatus.Active, GameStatus.Maintenance, GameStatus.Testing };

            var query = from bg in _tgsRepository.BrandGames.AsQueryable()
                        from game in _tgsRepository.Games
                        from sgp in _tgsRepository.SubGameProviders
                        from gp in _tgsRepository.GameProviders
                        where bg.BrandId == brandId
                            && bg.GameId == game.Id
                            && game.SubGameProviderCode == sgp.Code
                            && sgp.GameProviderId == gp.Id
                            && status.Contains(bg.Status)
                            && status.Contains(game.Status)
                        select new
                        {
                            game.Id,
                            game.Code,
                            game.Name,
                            game.Description,
                            game.SubGameProviderCode,
                            GpId = gp.Id,
                            GpCode = gp.Code,
                            game.Status,
                            game.Type,
                            game.PlatformType,
                            game.AvailableToBrand,
                            game.AvailableCurrencies,
                            game.Localization,
                            bg.Tag,
                            bg.DisplayOrder
                        };

            return query.ToAsyncEnumerable()
                .Select(info => new BrandGameInfo(
                    info.Id,
                    info.Code,
                    info.Name,
                    info.Description,
                    info.SubGameProviderCode,
                    info.GpId,
                    info.GpCode,
                    info.Type,
                    new TagInfo
                    {
                        New = info.Tag.IsNew(),
                        Hot = info.Tag.IsHot(),
                        Recommend = info.Tag.IsRecommend()
                    },
                    info.DisplayOrder,
                    string.IsNullOrEmpty(info.Localization)
                        ? new Dictionary<string, JsonDocument>()
                        : _jsonSerializationProvider.DeserializeFromString<Dictionary<string, JsonDocument>>(
                            info.Localization,
                            null)));
        }
    }
}
