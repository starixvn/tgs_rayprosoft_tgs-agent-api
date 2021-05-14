using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.GameProvider;
using TGS.Services.BackOffice.Application.Models.GameProviderBetLimit;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IGameProviderService
    {
        Task<GameProvider> GetGameProviderAsync(
            Expression<Func<GameProvider, bool>> predicate,
            Func<IQueryable<GameProvider>, IQueryable<GameProvider>> preQuery =
                null);

        IQueryable<GameProvider> GetGameProviderQueries(
            Func<IQueryable<GameProvider>, IQueryable<GameProvider>> preQuery = null,
            bool tracking = false);

        Task CreateGameProviderAsync(CreateGameProviderRequest request);
        Task UpdateGameProviderAsync(GameProvider gameProvider, UpdateGameProviderRequest request);
        Task ChangeStatusAsync(GameProvider gameProvider, UpdateGameProviderStatusRequest request);
        Task<bool> ExistsAsync(Expression<Func<GameProvider, bool>> predicate);
    }

    public class GameProviderService : BaseRepositoryOperations<ITgsRepository>, IGameProviderService
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly ITgsRepository _repository;

        public GameProviderService(ITgsRepository tgsRepository, IRedisCacheManager cacheManager) : base(tgsRepository)
        {
            _repository = tgsRepository;
            _cacheManager = cacheManager;
        }

        Task<GameProvider> IGameProviderService.GetGameProviderAsync(
            Expression<Func<GameProvider, bool>> predicate,
            Func<IQueryable<GameProvider>, IQueryable<GameProvider>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<GameProvider> IGameProviderService.GetGameProviderQueries(
            Func<IQueryable<GameProvider>, IQueryable<GameProvider>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        async Task IGameProviderService.UpdateGameProviderAsync(
            GameProvider gameProvider, UpdateGameProviderRequest request)
        {
            gameProvider.Name = request.Name;
            gameProvider.AutoCloseRound = request.AutoCloseRound;
            gameProvider.SupportTestPlayer = request.SupportTestPlayer;
            gameProvider.AllowKickPlayer = request.AllowKickPlayer;
            gameProvider.PlaceBetTimeout = request.PlaceBetTimeout;
            gameProvider.SettleBetTimeout = request.SettleBetTimeout;
            gameProvider.Description = request.Description;
            gameProvider.UpdatedByUsername = request.EditorUsername;
            gameProvider.UpdatedOn = DateTimeOffset.Now;

            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await DeleteEntitiesAsync(gameProvider.Currencies);
            await DeleteEntitiesAsync(gameProvider.Languages);

            gameProvider.Games = GenerateUpdateGames(gameProvider, request);
            gameProvider.Currencies = await GenerateCurrenciesAsync(gameProvider, request);
            gameProvider.Languages = await GenerateLanguagesAsync(gameProvider, request);
            gameProvider.BetLimits = GenerateBetLimits(gameProvider, request);

            await UpdateEntityAsync(gameProvider);

            await tran.CommitAsync();

            var cacheKeys = new[]
            {
                new CacheKey {CacheType = RedisCacheType.Currencies, Key = gameProvider.Id},
                new CacheKey {CacheType = RedisCacheType.Languages, Key = gameProvider.Id},
                new CacheKey {CacheType = RedisCacheType.GameProvider, Key = gameProvider.Id},
                new CacheKey {CacheType = RedisCacheType.GameProviderIdByCode, Key = gameProvider.Code},
                new CacheKey {CacheType = RedisCacheType.GameProviderConfiguration, Key = gameProvider.Id},
                new CacheKey {CacheType = RedisCacheType.GameProviderLanguages, Key = gameProvider.Id},
                new CacheKey {CacheType = RedisCacheType.GameProviderCurrencies, Key = gameProvider.Id},
                new CacheKey {CacheType = RedisCacheType.Games, Key = gameProvider.Id},
                new CacheKey {CacheType = RedisCacheType.SubGames, Key = gameProvider.Id},
                new CacheKey {CacheType = RedisCacheType.Localization, Key = gameProvider.Id}
            };

            await _cacheManager.ClearCacheByBatchAsync(cacheKeys);
        }

        async Task IGameProviderService.ChangeStatusAsync(GameProvider gameProvider,
            UpdateGameProviderStatusRequest request)
        {
            gameProvider.Status = request.Status;
            gameProvider.UpdatedByUsername = request.EditorUsername;
            gameProvider.UpdatedOn = DateTimeOffset.Now;

            await UpdateEntityAsync(gameProvider);
            await _cacheManager.ClearCacheAsync(RedisCacheType.GameProvider, gameProvider.Id);
        }

        async Task IGameProviderService.CreateGameProviderAsync(CreateGameProviderRequest request)
        {
            var gameProvider = new GameProvider
            {
                Code = request.Code,
                Name = request.Name,
                Status = Status.Active,
                AutoCloseRound = request.AutoCloseRound,
                SupportTestPlayer = request.SupportTestPlayer,
                AllowKickPlayer = request.AllowKickPlayer,
                PlaceBetTimeout = request.PlaceBetTimeout,
                SettleBetTimeout = request.SettleBetTimeout,
                UniqueId = request.UniqueId,
                Description = request.Description,
                CreatedByUsername = request.EditorUsername,
                UpdatedByUsername = request.EditorUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            gameProvider.Games = GenerateGames(gameProvider, request);
            gameProvider.Currencies = await GenerateCurrenciesAsync(gameProvider, request);
            gameProvider.Languages = await GenerateLanguagesAsync(gameProvider, request);
            gameProvider.BetLimits = GenerateBetLimits(gameProvider, request);

            await CreateEntityAsync(gameProvider);
        }

        Task<bool> IGameProviderService.ExistsAsync(Expression<Func<GameProvider, bool>> predicate) =>
            ExistsAsync(predicate);

        private static HashSet<Game> GenerateGames(GameProvider gameProvider, GameProviderRequest request)
        {
            var games = request.Games.Select((x, count) => new Game
            {
                Id = int.Parse($"{gameProvider.UniqueId}{count + 1:00000}"),
                Code = x.Code,
                Name = x.Name,
                Status = x.Status,
                ExternalId = x.ExternalId,
                Description = x.Description,
                EndpointPath = x.EndpointPath,
                RuleUrl = x.RuleUrl,
                GameProviderId = gameProvider.Id,
                Type = x.Type,
                PlatformType = x.PlatformType,
                SubGame = x.SubGame,
                HasTurnoverOnTieBet = x.HasTurnoverOnTieBet,
                Html5 = x.Html5,
                AvailableCurrencies = x.AvailableCurrencies,
                CreatedByUsername = request.EditorUsername,
                UpdatedByUsername = request.EditorUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            });
            return new(games);
        }

        private static HashSet<Game> GenerateUpdateGames(GameProvider gameProvider,
            GameProviderRequest request)
        {
            var maxNumberStr = gameProvider.Games.Max(x => x.Id).ToString();
            var maxNumber = int.Parse(maxNumberStr[^5..]);
            var games = request.Games.Select(x =>
            {
                var originGame = gameProvider.Games.FirstOrDefault(g => g.Id == x.Id);
                var id = originGame == null ? int.Parse($"{gameProvider.UniqueId}{maxNumber += 1:00000}") : x.Id;
                return new Game
                {
                    Id = id,
                    GameProviderId = gameProvider.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Status = x.Status,
                    ExternalId = x.ExternalId,
                    Description = x.Description,
                    EndpointPath = x.EndpointPath,
                    RuleUrl = x.RuleUrl,
                    SubGame = x.SubGame,
                    HasTurnoverOnTieBet = x.HasTurnoverOnTieBet,
                    Html5 = x.Html5,
                    AvailableCurrencies = x.AvailableCurrencies,
                    CreatedByUsername = originGame == null ? request.EditorUsername : x.CreatedByUsername,
                    UpdatedByUsername = originGame == null ? request.EditorUsername : x.UpdatedByUsername,
                    CreatedOn = originGame == null ? DateTimeOffset.Now : x.CreatedOn,
                    UpdatedOn = originGame == null ? DateTimeOffset.Now : x.UpdatedOn
                };
            });

            return new(games);
        }

        private async Task<HashSet<GameProviderCurrency>> GenerateCurrenciesAsync(GameProvider gameProvider,
            GameProviderRequest request)
        {
            var currencyCodes = request.Currencies.Select(x => x.Code);
            var currencies = await _repository.Currencies.AsQueryable().Where(x => currencyCodes.Contains(x.Code))
                .ToArrayAsync();
            var gameProviderCurrencies = currencies.Select(x => new GameProviderCurrency
            {
                GameProviderId = gameProvider.Id,
                CurrencyCode = x.Code,
                GameProviderCurrencyCode =
                    request.Currencies.First(c => c.Code == x.Code).GameProviderCurrencyCode
            });
            return new(gameProviderCurrencies);
        }

        private async Task<HashSet<GameProviderLanguage>> GenerateLanguagesAsync(GameProvider gameProvider,
            GameProviderRequest request)
        {
            var cultureCodes = request.Languages.Select(x => x.CultureCode);
            var languages = await _repository.Languages.AsQueryable().Where(x => cultureCodes.Contains(x.CultureCode))
                .ToArrayAsync();
            var gameProviderLanguages = languages.Select(x => new GameProviderLanguage
            {
                GameProviderId = gameProvider.Id,
                CultureCode = x.CultureCode,
                GameProviderCultureCode =
                    request.Languages.First(c => c.CultureCode == x.CultureCode).GameProviderCultureCode
            });
            return new(gameProviderLanguages);
        }

        private static HashSet<GameProviderBetLimit> GenerateBetLimits(GameProvider gameProvider,
            GameProviderRequest request)
        {
            GameProviderBetLimit Expression(GameProviderBetLimitModel x) =>
                new()
                {
                    Id = x.Id == Guid.Empty ? Guid.NewGuid() : x.Id,
                    GameProviderId = gameProvider.Id,
                    CurrencyCode = x.CurrencyCode,
                    MinBet = x.MinBet,
                    MaxBet = x.MaxBet,
                    MaxBetPerMatch = x.MaxBetPerMatch,
                    ExternalSetting = x.ExternalSetting
                };

            var betLimits = request.BetLimits.Select(Expression);

            return new(betLimits.ToArray());
        }
    }
}