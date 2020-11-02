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

        // TODO: Lock / Unlock / Batch Settle / Batch Cancel
    }

    public sealed partial class GamePlayActions : IGamePlayActions
    {
        async Task<GetBalanceResult> IGamePlayActions.GetBalanceAsync(string gameProviderCode, string playerCode)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(playerCode);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(gameProviderCode);

            var cur = await _currencyMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);

            var balance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false);

            return new GetBalanceResult
            {
                Balance = balance,
                CurrencyCode = cur
            };
        }

        async Task<BetResult> IGamePlayActions.PlaceBetAsync(BetData data)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode);
            await ValidatePlayerAsync(player, data.GameProviderId, data.GameProviderConfigurationId, data.PlatformType);
            await ThrowIfGameIsNotTakenByLicenseeAsync(player, data);
            data = await EnsureMappingsAsync(data, player.DefaultWalletCurrencyCode);
            var r = await _operations.PlaceBetAsync(data);
            return await EnsureMappingsAsync(data.GameProviderId, r);
        }

        async Task<BetResult> IGamePlayActions.SettleBetAsync(BetData data)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode);
            data = await EnsureMappingsAsync(data, player.DefaultWalletCurrencyCode);
            var res = await _operations.SettleBetAsync(data);
            return await EnsureMappingsAsync(data.GameProviderId, res);
        }

        async Task<BetResult> IGamePlayActions.CancelTransactionAsync(BetData data)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode);
            data = await EnsureMappingsAsync(data, player.DefaultWalletCurrencyCode);
            var res = await _operations.CancelTransactionAsync(data);
            return await EnsureMappingsAsync(data.GameProviderId, res);
        }

        private async Task ThrowIfGameIsNotTakenByLicenseeAsync(CachedPlayer player, BetData data,
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
                    $"gameProvider {data.GameProviderCode} doesn't support currency '{player.DefaultWalletCurrencyCode}'");
            }

            var platformType = data.PlatformType;
            var licenseeGames = await _licenseeOperations.GetLicenseeGamesAsync(player.LicenseeId, gameProviderId);
            var subGames = await _gameProviderOperations.GetSubGameIdsAsync(gameProviderId, platformType);

            var games = licenseeGames.Where(g => g.PlatformType == platformType)
                .Select(g => new IdMap {Id = g.GameId, ExternalId = g.ExternalGameId})
                .Concat(subGames).Distinct()
                .ToArray();

            var gameIds = games
                .Where(g => placeBetExternalGameIds.IgnoreCaseContains(g.ExternalId))
                .Select(g => g.Id)
                .ToArray();

            if (gameIds.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Licensee {player.LicenseeCode} doesn't have games '{placeBetExternalGameIds.JoinAsStringByComma()}'");
            }

            var validGameStatus = _validGameStatus.GetStatus(player.Type, type);
            foreach (var id in gameIds)
            {
                var cachedGame = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, id);
                if (cachedGame == null)
                {
                    throw new InvalidOperationException(
                        $"Licensee {player.LicenseeCode} doesn't have game with id '{id}'");
                }

                if (validGameStatus.Any(s => s == cachedGame.Status))
                {
                    continue;
                }

                if (cachedGame.Status == GameStatus.Maintenance)
                {
                    throw new GameUnderMaintenanceException(cachedGame.ExternalId);
                }

                throw new InvalidOperationException(
                    $"Licensee {player.LicenseeCode} doesn't have game '{cachedGame.ExternalId}' with status '{validGameStatus.JoinAsStringByComma()}' but game status is '{cachedGame.Status}'");
            }
        }

        private async Task<BetData> EnsureMappingsAsync(BetData data, string playerCurrencyCode)
        {
            if (!data.HasCurrencyFromGameProvider)
            {
                return data;
            }

            foreach (var t in data.Transactions)
            {
                if (t.GameActionType != GameActionType.PlaceBet)
                    continue;

                var internalCurrency =
                    await _currencyMapper.ExternalToInternalAsync(t.CurrencyCode, data.GameProviderId);

                if (internalCurrency.IgnoreCaseNotEquals(playerCurrencyCode))
                {
                    throw new CurrencyNotMatchingException(playerCurrencyCode, internalCurrency);
                }
            }

            return data;
        }

        private async Task<BetResult> EnsureMappingsAsync(Guid gameProviderId, BetResult r)
        {
            var currencyCode = await _currencyMapper.InternalToExternalAsync(r.CurrencyCode, gameProviderId);
            return r with { CurrencyCode = currencyCode };
        }
    }
}