using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Infrastructures.Extensions;

namespace TGS.Services.GameServices
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
            await ThrowIfGameIsNotTakenByBrandAsync(player, data);

            return null;
        }

        Task<BetResult> IGamePlayActions.SettleBetAsync(BetData data)
        {
            throw new NotImplementedException();
        }

        Task<BetResult> IGamePlayActions.CancelTransactionAsync(BetData data)
        {
            throw new NotImplementedException();
        }

        private async Task ThrowIfGameIsNotTakenByBrandAsync(CachedPlayer player, BetData data, GameActionType type = GameActionType.PlaceBet)
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
            var brandGames = await _brandOperations.GetBrandGamesAsync(player.BrandId, gameProviderId);

            var gameIds = brandGames.Where(g => placeBetExternalGameIds.IgnoreCaseContains(g.ExternalGameId) &&
                                            platformTypes.Contains(g.PlatformType))
                                    .Select(g => g.GameId)
                                    .ToArray();

            if (gameIds.Length == 0)
            {
                throw new InvalidOperationException($"Brand {player.BrandCode} doesn't have games '{placeBetExternalGameIds.JoinAsStringByComma()}'");
            }

            var validGameStatus = _validGameStatus.GetStatus(player.Type, type);
            foreach (var id in gameIds)
            {
                var cachedGame = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, id);
                if (cachedGame == null)
                {
                    throw new InvalidOperationException($"Brand {player.BrandCode} doesn't have game with id '{id}'");
                }

                if (validGameStatus.All(s => s != cachedGame.Status))
                {
                    throw new InvalidOperationException($"Brand {player.BrandCode} doesn't have game '{cachedGame.ExternalId}' with status '{validGameStatus.JoinAsStringByComma()}'");
                }
            }
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