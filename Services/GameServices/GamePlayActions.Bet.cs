using System;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;

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

        Task<BetResult> IGamePlayActions.PlaceBetAsync(BetData data)
        {
            throw new NotImplementedException();
        }

        Task<BetResult> IGamePlayActions.SettleBetAsync(BetData data)
        {
            throw new NotImplementedException();
        }
           
        Task<BetResult> IGamePlayActions.CancelTransactionAsync(BetData data)
        {
            throw new NotImplementedException();
        }
    }
}