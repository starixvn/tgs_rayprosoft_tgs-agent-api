using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.GameOperations
{
    public partial interface IGamePlayActions
    {
        Task<GetBalanceResult> GetBalanceAsync(string gameProviderCode, string playerCode);

        Task<BetResult> PlaceBetAsync(BetData data);

        Task<BetResult> SettleBetAsync(BetData data);

        Task<BetResult> CancelTransactionAsync(BetData data);

        Task<BetResult> PlaceAndSettleBetAsync(BetData data);

        // TODO: Lock / Unlock
    }

    public sealed partial class GamePlayActions : IGamePlayActions
    {
        async Task<GetBalanceResult> IGamePlayActions.GetBalanceAsync(string gameProviderCode, string playerCode)
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

        Task<BetResult> IGamePlayActions.PlaceBetAsync(BetData data) =>
            ExecuteGamePlayActionAsync(data, _operations.PlaceBetAsync, async player =>
            {
                await ValidatePlayerAsync(player, data.GameProviderId, data.GameProviderConfigurationId,
                    data.PlatformType);

                await ThrowIfGameIsNotTakenByBrandAsync(player, data);
            });

        Task<BetResult> IGamePlayActions.SettleBetAsync(BetData data) =>
            ExecuteGamePlayActionAsync(data, _operations.SettleBetAsync);

        Task<BetResult> IGamePlayActions.CancelTransactionAsync(BetData data) =>
            ExecuteGamePlayActionAsync(data, _operations.CancelTransactionAsync);

        Task<BetResult> IGamePlayActions.PlaceAndSettleBetAsync(BetData data) =>
            ExecuteGamePlayActionAsync(data, _operations.PlaceAndSettleBetAsync, async player =>
            {
                await ValidatePlayerAsync(player, data.GameProviderId, data.GameProviderConfigurationId,
                    data.PlatformType);

                await ThrowIfGameIsNotTakenByBrandAsync(player, data);
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

            if (!platformType.HasValue)
            {
                throw new InvalidOperationException("Platform can't be empty for place bet action");
            }

            var gamePlatformTypes = GetGamePlatformTypes();
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

            GamePlatformType[] GetGamePlatformTypes() => platformType switch
            {
                PlatformType.Desktop => new[] {GamePlatformType.Desktop, GamePlatformType.DesktopAndMobile},
                PlatformType.Mobile  => new[] {GamePlatformType.Mobile, GamePlatformType.DesktopAndMobile}
            };
        }

        private async Task<BetData> EnsureMappingsAsync(BetData data, string playerCurrencyCode)
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

        private async Task<BetResult> EnsureMappingsAsync(Guid gameProviderId, BetResult r)
        {
            var currencyCode = await _currencyMapper.InternalToExternalAsync(r.CurrencyCode, gameProviderId);
            return r with {GameProviderCurrencyCode = currencyCode};
        }

        private async Task<BetResult> ExecuteGamePlayActionAsync(BetData data, Func<BetData, Task<BetResult>> execute,
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