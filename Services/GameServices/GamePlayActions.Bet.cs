using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;

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

            return new GetBalanceResult(balance, cur);
        }

        async Task<BetResult> IGamePlayActions.PlaceBetAsync(BetData data)
        {
            var player =await _playerOperations.GetCachedPlayerAsync(data.PlayerCode);
            await ValidtePlayerAysnc(player, data.GameProviderId, data.GameProviderConfigurationId);
            await ThrowIfGameIsNotTakenByBrandAsync(player, data.GameProviderId, data.GameProviderCode);
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

        private async Task ThrowIfGameIsNotTakenByBrandAsync(CachedPlayer player, Guid gameProviderId, string gameProviderCode, GameActionType type = GameActionType.PlaceBet)
        {
            if (type != GameActionType.PlaceBet)
                return;

            var supported = await _gameProviderOperations.CurrencySupportedAsync(gameProviderId, player.DefaultWalletCurrencyCode);
            if (!supported) 
            { 
                throw new InvalidOperationException($"gameProvider {gameProviderId} doesn't support currency '{player.DefaultWalletCurrencyCode}'");
            }

            //var loginRecord = await _playerOperations.GetPlayerLatestEnabledLoginRecordAsync(player.PlayerCode);
            //var validGameStatus = _validGameStatus.GetStatus(player.Type, type);
            //var brandGames = await _brandOperations.GetBrandGamesAsync(player.BrandId, gameProviderId);


        }

    }
}