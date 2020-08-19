using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.Models.Services;
using TGS.Core.Models.Services.Games;
using TGS.Infrastructures.Extensions;
using TGS.Services.Operations;

namespace TGS.Services.GameServices
{
    public interface IGamePlayActions
    {
        Task ValidatePlayerAsync();

        Task<GetBalanceResult> GetBalanceAsync(string gameProviderCode, string playerCode);

        Task<BetResult> LockAmountAsync(BetData data);

        Task<BetResult> PlaceBetAsync(BetData data);

        Task<BetResult> SettleBetAsync(BetData data);

        Task<BetResult> CancelTransactionAsync(BetData data);
    }

    public sealed class GamePlayActions : IGamePlayActions
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IGameProviderOperations _gameProviderOperations;

        public GamePlayActions(
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations)
        {
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
        }

        Task<BetResult> IGamePlayActions.CancelTransactionAsync(BetData data)
        {
            throw new System.NotImplementedException();
        }

        async Task<GetBalanceResult> IGamePlayActions.GetBalanceAsync(string gameProviderCode, string playerCode)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(playerCode);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(gameProviderCode);

            //var cur = await _gpCurrencyMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);

            //var balance = await GetWalletBalanceAsync(gameProviderId, player, forceUpdate);

            return new GetBalanceResult
            {
                //Balance = balance,
                //CurrencyCode = cur
            };
        }

        Task<BetResult> IGamePlayActions.LockAmountAsync(BetData data)
        {
            throw new System.NotImplementedException();
        }

        Task<BetResult> IGamePlayActions.PlaceBetAsync(BetData data)
        {
            throw new System.NotImplementedException();
        }

        Task<BetResult> IGamePlayActions.SettleBetAsync(BetData data)
        {
            throw new System.NotImplementedException();
        }

        Task IGamePlayActions.ValidatePlayerAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
