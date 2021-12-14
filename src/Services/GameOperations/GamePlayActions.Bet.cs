using System;
using System.Collections.Generic;
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
        ValueTask<BetResult> CancelTransactionAsync(BetData data);

        ValueTask<GetBalanceResult> GetBalanceAsync(string gameProviderCode, string playerCode);

        ValueTask<BetResult> LockAmountAsync(BetData data);

        ValueTask<BetResult> PlaceAndSettleBetAsync(BetData data);

        ValueTask<BetResult> PlaceBetAsync(BetData data);

        ValueTask<BetResult> SettleBetAsync(BetData data);
    }

    public sealed partial class GamePlayActions : IGamePlayActions
    {
        public ValueTask<BetResult> CancelTransactionAsync(BetData data)
            => ExecuteGamePlayActionAsync(data, _operations.CancelTransactionAsync);

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

        public ValueTask<BetResult> PlaceAndSettleBetAsync(BetData data)
            => ExecuteGamePlayActionAsync(data,
                _operations.PlaceAndSettleBetAsync,
                player => ValidateRequiredStatusOnBettingAsync(data, player));

        public ValueTask<BetResult> PlaceBetAsync(BetData data)
                    => ExecuteGamePlayActionAsync(data,
                _operations.PlaceAndSettleBetAsync,
                player => ValidateRequiredStatusOnBettingAsync(data, player));

        public ValueTask<BetResult> SettleBetAsync(BetData data)
            => ExecuteGamePlayActionAsync(data, _operations.SettleBetAsync);

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

        private void ValidateGameStatus(
            Guid brandId,
            IEnumerable<GameInfo> games,
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

            var placeBetExternalGameIds = data.Transactions
                .Select(t => t.ExternalGameId).Distinct().ToArray();

            var games = await _gameAccessor
                .GetBrandAvailableGamesAsync(
                    player.BrandId,
                    data.GameProviderId,
                    placeBetExternalGameIds,
                    gamePlatformTypes
                ).ConfigureAwait(false);

            ValidateGameStatus(player.BrandId, games, player.Type);
        }
    }
}
