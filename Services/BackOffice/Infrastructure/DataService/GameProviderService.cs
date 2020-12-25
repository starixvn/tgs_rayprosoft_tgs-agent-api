using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.GameProvider;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IGameProviderService
    {
        Task<GameProvider> GetGameProviderAsync(
            Expression<Func<GameProvider, bool>> predicate,
            Func<IQueryable<GameProvider>, IQueryable<GameProvider>> preQuery =
                null);

        IQueryable<GameProvider> GetGameProvidersAsync(
            Func<IQueryable<GameProvider>, IQueryable<GameProvider>> preQuery = null,
            bool tracking = false);

        Task CreateGameProviderAsync(CreateGameProviderRequest request);
        Task UpdateGameProviderAsync(GameProvider gameProvider, UpdateGameProviderRequest request);
        Task ChangeStatusAsync(GameProvider gameProvider, UpdateGameProviderStatusRequest request);
    }

    public class GameProviderService : BaseService, IGameProviderService
    {
        private readonly ITgsRepository _repository;

        public GameProviderService(ITgsRepository tgsRepository) : base(tgsRepository)
        {
            _repository = tgsRepository;
        }

        Task<GameProvider> IGameProviderService.GetGameProviderAsync(
            Expression<Func<GameProvider, bool>> predicate,
            Func<IQueryable<GameProvider>, IQueryable<GameProvider>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<GameProvider> IGameProviderService.GetGameProvidersAsync(
            Func<IQueryable<GameProvider>, IQueryable<GameProvider>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        async Task IGameProviderService.UpdateGameProviderAsync(
            GameProvider gameProvider, UpdateGameProviderRequest request)
        {
            var (games, deleteGameIds) = GenerateUpdateGames(gameProvider, request);

            gameProvider.Name = request.Name;
            gameProvider.AutoCloseRound = request.AutoCloseRound;
            gameProvider.SupportTestPlayer = request.SupportTestPlayer;
            gameProvider.AllowKickPlayer = request.AllowKickPlayer;
            gameProvider.PlaceBetTimeout = request.PlaceBetTimeout;
            gameProvider.SettleBetTimeout = request.SettleBetTimeout;
            gameProvider.Description = request.Description;
            gameProvider.UpdatedByUsername = request.Username;
            gameProvider.UpdatedOn = DateTimeOffset.Now;

            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await DeleteEntitiesAsync(_repository.Games.Where(x => deleteGameIds.Contains(x.Id)));
            await DeleteEntitiesAsync(gameProvider.Currencies);
            await DeleteEntitiesAsync(gameProvider.Languages);

            gameProvider.Games = games;
            gameProvider.Currencies = await GenerateCurrencies(gameProvider, request);
            gameProvider.Languages = await GenerateLanguages(gameProvider, request);

            await CreateEntityAsync(GenerateNewBetLimits(gameProvider, request));

            await UpdateEntityAsync(gameProvider);

            await tran.CommitAsync();
        }

        Task IGameProviderService.ChangeStatusAsync(GameProvider gameProvider, UpdateGameProviderStatusRequest request)
        {
            gameProvider.Status = request.Status;
            gameProvider.UpdatedByUsername = request.Username;
            gameProvider.UpdatedOn = DateTimeOffset.UtcNow;

            return UpdateEntityAsync(gameProvider);
        }

        async Task IGameProviderService.CreateGameProviderAsync(CreateGameProviderRequest request)
        {
            var gameProvider = new GameProvider
            {
                Id = Guid.NewGuid(),
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
                CreatedByUsername = request.Username,
                UpdatedByUsername = request.Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            gameProvider.Games = GenerateGames(gameProvider, request);
            gameProvider.Currencies = await GenerateCurrencies(gameProvider, request);
            gameProvider.Languages = await GenerateLanguages(gameProvider, request);
            gameProvider.BetLimits = GenerateBetLimits(gameProvider, request);

            await CreateEntityAsync(gameProvider);
        }

        private static HashSet<Game> GenerateGames(GameProvider gameProvider, CreateGameProviderRequest request)
        {
            var games = request.Games.Select((x, count) => new Game
            {
                Id = int.Parse($"{gameProvider.UniqueId}{count + 1:0000}"),
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
                CreatedByUsername = request.Username,
                UpdatedByUsername = request.Username,
                CreatedOn = DateTimeOffset.UtcNow,
                UpdatedOn = DateTimeOffset.Now
            });
            return new HashSet<Game>(games);
        }

        private static (HashSet<Game>, IEnumerable<int> deleteGameIds) GenerateUpdateGames(GameProvider gameProvider,
            UpdateGameProviderRequest request)
        {
            var newGames = request.Games.Where(x => x.Id == 0);
            var deleteGameIds = gameProvider.Games.Select(x => x.Id).Except(request.Games.Select(x => x.Id));
            var maxNumber = gameProvider.Games.Max(x => x.Id);
            var games = request.Games.Select(x =>
            {
                var id = x.Id == 0 ? int.Parse($"{gameProvider.UniqueId}{maxNumber++:0000}") : x.Id;
                return new Game
                {
                    Id = id,
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
                    CreatedByUsername = x.Id == 0 ? request.Username : x.CreatedByUsername,
                    UpdatedByUsername = x.Id == 0 ? request.Username : x.UpdatedByUsername,
                    CreatedOn = x.Id == 0 ? DateTimeOffset.UtcNow : x.CreatedOn,
                    UpdatedOn = x.Id == 0 ? DateTimeOffset.Now : x.UpdatedOn
                };
            });

            return (new HashSet<Game>(games), deleteGameIds);
        }

        private async Task<HashSet<GameProviderCurrency>> GenerateCurrencies(GameProvider gameProvider,
            GameProviderRequest request)
        {
            var currencyCodes = request.Currencies.Select(x => x.CurrencyCode);
            var currencies = await _repository.Currencies.Where(x => currencyCodes.Contains(x.Code)).ToArrayAsync();
            var gameProviderCurrencies = currencies.Select(x => new GameProviderCurrency
            {
                GameProviderId = gameProvider.Id,
                CurrencyCode = x.Code,
                GameProviderCurrencyCode =
                    request.Currencies.First(c => c.CurrencyCode == x.Code).GameProviderCurrencyCode
            });
            return new HashSet<GameProviderCurrency>(gameProviderCurrencies);
        }

        private async Task<HashSet<GameProviderLanguage>> GenerateLanguages(GameProvider gameProvider,
            GameProviderRequest request)
        {
            var cultureCodes = request.Languages.Select(x => x.CultureCode);
            var languages = await _repository.Languages.Where(x => cultureCodes.Contains(x.CultureCode)).ToArrayAsync();
            var gameProviderLanguages = languages.Select(x => new GameProviderLanguage
            {
                GameProviderId = gameProvider.Id,
                CultureCode = x.CultureCode,
                GameProviderCultureCode =
                    request.Languages.First(c => c.CultureCode == x.CultureCode).GameProviderCultureCode
            });
            return new HashSet<GameProviderLanguage>(gameProviderLanguages);
        }

        private HashSet<GameProviderBetLimit> GenerateBetLimits(GameProvider gameProvider,
            GameProviderRequest request)
        {
            var gameProviderBetLimits = request.BetLimits.Select(x => new GameProviderBetLimit
            {
                GameProviderId = gameProvider.Id,
                CurrencyCode = x.CurrencyCode,
                MinBet = x.MinBet,
                MaxBet = x.MaxBet,
                MaxBetPerMatch = x.MaxBetPerMatch,
                ExternalSetting = x.ExternalSetting
            });
            return new HashSet<GameProviderBetLimit>(gameProviderBetLimits);
        }

        private HashSet<GameProviderBetLimit> GenerateNewBetLimits(GameProvider gameProvider,
            GameProviderRequest request)
        {
            var gameProviderBetLimits = request.BetLimits.Where(x => x.Id.Equals(Guid.Empty)).Select(x =>
                new GameProviderBetLimit
                {
                    GameProviderId = gameProvider.Id,
                    CurrencyCode = x.CurrencyCode,
                    MinBet = x.MinBet,
                    MaxBet = x.MaxBet,
                    MaxBetPerMatch = x.MaxBetPerMatch,
                    ExternalSetting = x.ExternalSetting
                });
            return new HashSet<GameProviderBetLimit>(gameProviderBetLimits);
        }
    }
}