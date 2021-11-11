using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations
{
    public partial interface IGamePlayActions
    {
        ValueTask<GetBalanceResult> GetBalanceAsync(string gameProviderCode, string playerCode);

        ValueTask<BetResult> LockAmountAsync(BetData data);

        ValueTask<BetResult> PlaceBetAsync(BetData data);

        ValueTask<BetResult> SettleBetAsync(BetData data);

        ValueTask<BetResult> CancelTransactionAsync(BetData data);

        ValueTask<BetResult> PlaceAndSettleBetAsync(BetData data);
    }

    public sealed partial class GamePlayActions : IGamePlayActions
    {
        public async ValueTask<GetBalanceResult> GetBalanceAsync(string gameProviderCode, string playerCode)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(playerCode);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(gameProviderCode);

            var cur = await _currencyMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);
            var balance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false);

            return new()
            {
                Username = player.Username,
                Balance = balance,
                GameProviderCurrencyCode = cur
            };
        }

        public ValueTask<BetResult> LockAmountAsync(BetData data)
            => ExecuteGamePlayActionAsync(data,
                _operations.PlaceAndSettleBetAsync,
                player => ValidateRequiredStatusOnBettingAsync(data, player));

        public ValueTask<BetResult> PlaceBetAsync(BetData data)
            => ExecuteGamePlayActionAsync(data,
                _operations.PlaceAndSettleBetAsync,
                player => ValidateRequiredStatusOnBettingAsync(data, player));

        public ValueTask<BetResult> SettleBetAsync(BetData data)
            => ExecuteGamePlayActionAsync(data, _operations.SettleBetAsync);

        public ValueTask<BetResult> CancelTransactionAsync(BetData data)
            => ExecuteGamePlayActionAsync(data, _operations.CancelTransactionAsync);

        public ValueTask<BetResult> PlaceAndSettleBetAsync(BetData data)
            => ExecuteGamePlayActionAsync(data,
                _operations.PlaceAndSettleBetAsync,
                player => ValidateRequiredStatusOnBettingAsync(data, player));

        private async ValueTask ValidateRequiredStatusOnBettingAsync(BetData data, CachedPlayer player)
        {
            if (data.IgnorePlaceBetValidation)
            {
                return;
            }

            await ValidateRequiredStatusAsync(player, data.GameProviderId, data.GameProviderConfigurationId)
                .ConfigureAwait(false);

            await ValidateGameProviderSupportedCurrencyAsync(data.GameProviderId, player.DefaultWalletCurrencyCode);

            var gamePlatformTypes = data.AllowPlaceBetWithDifferentPlatformToken || !data.PlatformType.HasValue
                ? Consts.AllGamePlatformTypes
                : data.PlatformType!.Value.ToGamePlatformTypes();

            var games = await GetBrandAvailableGamesAsync(
                player.BrandId,
                data.GameProviderId,
                data.Transactions,
                gamePlatformTypes).ConfigureAwait(false);

            ValidateGameStatus(player.BrandId, games, player.Type);
        }

        private async ValueTask<GameInfo[]> GetBrandAvailableGamesAsync(
            Guid brandId,
            Guid gameProviderId,
            TransactionData[] transactions,
            GamePlatformType[] gamePlatformTypes)
        {
            var placeBetExternalGameIds = transactions.Select(t => t.ExternalGameId).Distinct().ToArray();

            if (placeBetExternalGameIds.Length == 0)
            {
                throw new InvalidOperationException("No transaction is found when getting brand available games");
            }

            var brandGames =
                await _brandOperations.GetBrandGamesAsync(brandId, gameProviderId, gamePlatformTypes)
                    .ConfigureAwait(false);

            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, gamePlatformTypes)
                .ConfigureAwait(false);

            var subGames = await _gameProviderOperations.GetSubGamesAsync(gameProviderId, gamePlatformTypes)
                .ConfigureAwait(false);

            var gameInfo = from bg in brandGames
                           from g in cachedGames
                           where bg.GameId == g.Id
                           let gameStatus = GetGameStatus(bg.BrandGameStatus, g.Status)
                           select new GameInfo(g.Id, g.ExternalId, gameStatus, g.SubGame, g.AvailableToBrand);

            var subGameInfo = from sg in subGames
                              from g in cachedGames
                              where sg.Id == g.Id
                              select new GameInfo(g.Id, g.ExternalId, g.Status, g.SubGame, g.AvailableToBrand);

            var games = gameInfo.Concat(subGameInfo).Distinct()
                .Where(g => placeBetExternalGameIds.Contains(g.ExternalId)).ToArray();

            if (games.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Brand '{brandId}' doesn't have games '{placeBetExternalGameIds.JoinAsStringByComma()}'");
            }

            return games;
        }

        private void ValidateGameStatus(
            Guid brandId,
            GameInfo[] games,
            PlayerType playerType)
        {
            var gameStatuses = _validGameStatus.GetStatus(playerType);

            foreach (var game in games)
            {
                if (gameStatuses.Contains(game.Status))
                {
                    continue;
                }

                if (game.Status == GameStatus.Maintenance)
                {
                    throw new GameUnderMaintenanceException(game.ExternalId);
                }

                throw new InvalidOperationException(
                    $"Brand '{brandId}' doesn't have game '{game.ExternalId}' with status '{gameStatuses.JoinAsStringByComma()}' but game status is '{game.Status}'");
            }
        }

        private static GameStatus GetGameStatus(GameStatus brandGameStatus, GameStatus gameStatus)
        {
            if (brandGameStatus == GameStatus.Inactive || gameStatus == GameStatus.Inactive)
            {
                return GameStatus.Inactive;
            }

            if (brandGameStatus == GameStatus.Testing || gameStatus == GameStatus.Testing)
            {
                return GameStatus.Testing;
            }

            if (brandGameStatus == GameStatus.Maintenance || gameStatus == GameStatus.Maintenance)
            {
                return GameStatus.Maintenance;
            }

            return GameStatus.Active;
        }

        private async ValueTask ValidateGameProviderSupportedCurrencyAsync(
            Guid gameProviderId, string playerCurrencyCode)
        {
            var supported =
                await _gameProviderOperations.CurrencySupportedAsync(gameProviderId, playerCurrencyCode);

            if (!supported)
            {
                throw new InvalidOperationException(
                    $"GameProvider '{gameProviderId}' doesn't support currency '{playerCurrencyCode}'");
            }
        }

        private async ValueTask<BetData> EnsureMappingsAsync(BetData data, string playerCurrencyCode)
        {
            if (!data.HasCurrencyFromGameProvider)
            {
                return data;
            }

            var internalCurrency =
                await _currencyMapper.ExternalToInternalAsync(data.CurrencyCode, data.GameProviderId);

            if (internalCurrency.IgnoreCaseNotEquals(playerCurrencyCode))
            {
                throw new CurrencyNotMatchingException(playerCurrencyCode, internalCurrency);
            }

            return data;
        }

        private async ValueTask<BetResult> EnsureMappingsAsync(Guid gameProviderId, BetResult r)
        {
            var currencyCode = await _currencyMapper.InternalToExternalAsync(r.CurrencyCode, gameProviderId);

            return r with
            {
                GameProviderCurrencyCode = currencyCode
            };
        }

        private async ValueTask<BetResult> ExecuteGamePlayActionAsync(
            BetData data,
            Func<BetData, ValueTask<BetResult>> execute,
            Func<CachedPlayer, ValueTask> preAction = null)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode);

            if (preAction != null)
            {
                await preAction(player).ConfigureAwait(false);
            }

            data = await EnsureMappingsAsync(data, player.DefaultWalletCurrencyCode);

            var r = await execute(data).ConfigureAwait(false);

            return await EnsureMappingsAsync(data.GameProviderId, r);
        }
    }
}
