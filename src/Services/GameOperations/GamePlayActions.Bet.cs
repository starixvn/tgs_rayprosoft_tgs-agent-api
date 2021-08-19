using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;

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
        async ValueTask<GetBalanceResult> IGamePlayActions.GetBalanceAsync(string gameProviderCode, string playerCode)
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

        ValueTask<BetResult> IGamePlayActions.LockAmountAsync(BetData data) =>
            ExecuteGamePlayActionAsync(data, _operations.LockAmountAsync, async player =>
            {
                var platformType = data.AllowPlaceBetWithDifferentPlatformToken ? null : data.PlatformType;
                await ValidatePlayerAsync(player, data.GameProviderId, data.GameProviderConfigurationId,
                    platformType);

                await ThrowIfGameIsNotTakenByBrandAsync(player, data);
            });

        ValueTask<BetResult> IGamePlayActions.PlaceBetAsync(BetData data) =>
            ExecuteGamePlayActionAsync(data, _operations.PlaceBetAsync, async player =>
            {
                var platformType = data.AllowPlaceBetWithDifferentPlatformToken ? null : data.PlatformType;
                await ValidatePlayerAsync(player, data.GameProviderId, data.GameProviderConfigurationId,
                    platformType).ConfigureAwait(false);

                await ThrowIfGameIsNotTakenByBrandAsync(player, data).ConfigureAwait(false);
                ;
            });

        ValueTask<BetResult> IGamePlayActions.SettleBetAsync(BetData data) =>
            ExecuteGamePlayActionAsync(data, _operations.SettleBetAsync);

        ValueTask<BetResult> IGamePlayActions.CancelTransactionAsync(BetData data) =>
            ExecuteGamePlayActionAsync(data, _operations.CancelTransactionAsync);

        ValueTask<BetResult> IGamePlayActions.PlaceAndSettleBetAsync(BetData data) =>
            ExecuteGamePlayActionAsync(data, _operations.PlaceAndSettleBetAsync, async player =>
            {
                var platformType = data.AllowPlaceBetWithDifferentPlatformToken ? null : data.PlatformType;
                await ValidatePlayerAsync(player, data.GameProviderId, data.GameProviderConfigurationId,
                    platformType).ConfigureAwait(false);

                await ThrowIfGameIsNotTakenByBrandAsync(player, data)
                    .ConfigureAwait(false);
            });

        private async Task ThrowIfGameIsNotTakenByBrandAsync(CachedPlayer player, BetData data,
            GameActionType type = GameActionType.PlaceBet)
        {
            if (type != GameActionType.PlaceBet)
            {
                return;
            }

            var placeBetExternalGameIds = data.Transactions.Select(t => t.ExternalGameId).Distinct().ToArray();

            var gameProviderId = data.GameProviderId;
            var supported =
                await _gameProviderOperations.CurrencySupportedAsync(gameProviderId, player.DefaultWalletCurrencyCode);

            if (!supported)
            {
                throw new InvalidOperationException(
                    $"GameProvider '{data.GameProviderCode}' doesn't support currency '{player.DefaultWalletCurrencyCode}'");
            }

            var platformType = data.PlatformType;
            var gamePlatformTypes = data.AllowPlaceBetWithDifferentPlatformToken
                ? Consts.AllGamePlatformTypes
                : platformType!.Value.ToGamePlatformTypes();

            var brandGames =
                await _brandOperations.GetBrandGamesAsync(player.BrandId, gameProviderId, gamePlatformTypes);
            var subGames = await _gameProviderOperations.GetSubGamesAsync(gameProviderId, gamePlatformTypes);

            var games = brandGames.Select(g => new IdMap<int>
                {
                    Id = g.GameId,
                    ExternalId = g.ExternalGameId
                })
                .Concat(subGames.Select(s => new IdMap<int>
                {
                    Id = s.Id,
                    ExternalId = s.ExternalId
                })).Distinct()
                .ToArray();

            var gameIds = games
                .Where(g => placeBetExternalGameIds.IgnoreCaseContains(g.ExternalId))
                .Select(g => g.Id)
                .ToArray();

            if (gameIds.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Brand {player.BrandCode} doesn't have games '{placeBetExternalGameIds.JoinAsStringByComma()}'");
            }

            var gameStatuses = _validGameStatus.GetStatus(player.Type, type);

            foreach (var id in gameIds)
            {
                var cachedGame = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, id);
                if (cachedGame == null)
                {
                    throw new InvalidOperationException(
                        $"Brand {player.BrandCode} doesn't have game with id '{id}'");
                }

                if (gameStatuses.Any(s => s == cachedGame.Status))
                {
                    continue;
                }

                if (cachedGame.Status == GameStatus.Maintenance)
                {
                    throw new GameUnderMaintenanceException(cachedGame.ExternalId);
                }

                throw new InvalidOperationException(
                    $"Brand {player.BrandCode} doesn't have game '{cachedGame.ExternalId}' with status '{gameStatuses.JoinAsStringByComma()}' but game status is '{cachedGame.Status}'");
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
            return r with { GameProviderCurrencyCode = currencyCode };
        }

        private async ValueTask<BetResult> ExecuteGamePlayActionAsync(BetData data,
            Func<BetData, ValueTask<BetResult>> execute,
            Func<CachedPlayer, Task> preAction = null)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode);

            if (preAction != null)
            {
                await preAction(player);
            }

            data = await EnsureMappingsAsync(data, player.DefaultWalletCurrencyCode);

            var r = await execute(data);

            return await EnsureMappingsAsync(data.GameProviderId, r);
        }
    }
}