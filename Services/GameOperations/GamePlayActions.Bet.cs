using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Infrastructures.Extensions;

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
            await ValidtePlayerAysnc(player, data.GameProviderId, data.GameProviderConfigurationId);
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

        private async Task ThrowIfGameIsNotTakenByLicenseeAsync(CachedPlayer player, BetData data, GameActionType type = GameActionType.PlaceBet)
        {
            if (type != GameActionType.PlaceBet)
                return;

            var placeBetExternalGameIds = data.Transactions.Select(t => t.ExternalGameId).Distinct().ToArray();

            var gameProviderId = data.GameProviderId;
            var supported = await _gameProviderOperations.CurrencySupportedAsync(gameProviderId, player.DefaultWalletCurrencyCode);
            if (!supported)
            {
                throw new InvalidOperationException($"gameProvider {data.GameProviderCode} doesn't support currency '{player.DefaultWalletCurrencyCode}'");
            }

            var platformTypes = await GetPlatformTypes(player.PlayerCode, data.PlatformType);
            var licenseeGames = await _licenseeOperations.GetLicenseeGamesAsync(player.LicenseeId, gameProviderId);

            var gameIds = licenseeGames.Where(g => placeBetExternalGameIds.IgnoreCaseContains(g.ExternalGameId) &&
                                            platformTypes.Contains(g.PlatformType))
                                    .Select(g => g.GameId)
                                    .ToArray();

            if (gameIds.Length == 0)
            {
                throw new InvalidOperationException($"Licensee {player.LicenseeCode} doesn't have games '{placeBetExternalGameIds.JoinAsStringByComma()}'");
            }

            var validGameStatus = _validGameStatus.GetStatus(player.Type, type);
            foreach (var id in gameIds)
            {
                var cachedGame = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, id);
                if (cachedGame == null)
                {
                    throw new InvalidOperationException($"Licensee {player.LicenseeCode} doesn't have game with id '{id}'");
                }

                if (validGameStatus.All(s => s != cachedGame.Status))
                {
                    throw new InvalidOperationException($"Licensee {player.LicenseeCode} doesn't have game '{cachedGame.ExternalId}' with status '{validGameStatus.JoinAsStringByComma()}'");
                }
            }
        }

        private async Task<BetData> EnsureMappingsAsync(BetData data, string playerCurrencyCode)
        {
            if (!data.HasCurrencyFromGameProvider)
                return data;

            foreach (var t in data.Transactions)
            {
                if (t.GameActionType != GameActionType.PlaceBet)
                    continue;

                var internalCurrency = await _currencyMapper.ExternalToInternalAsync(t.CurrencyCode, data.GameProviderId);
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

        private async Task<PlatformType[]> GetPlatformTypes(string playerCode, PlatformType? platformType)
        {
            if (platformType.HasValue)
            {
                return new[] { platformType.Value };
            }

            var loginRecords = await _playerOperations.GetPlayerEnabledLoginRecordsAsync(playerCode);
            return loginRecords.Select(r => r.PlatformType).ToArray();
        }
    }
}