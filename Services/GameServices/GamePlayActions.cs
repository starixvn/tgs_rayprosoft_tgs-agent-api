using System;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;

namespace TGS.Services.GameServices
{
    public interface IGamePlayActions
    {
        Task<PlayerValidationResult> ValidatePlayerAsync(PlayerValidationData data);

        Task<GetBalanceResult> GetBalanceAsync(string gameProviderCode, string playerCode);

        Task<BetResult> LockAmountAsync(BetData data);

        Task<BetResult> PlaceBetAsync(BetData data);

        Task<BetResult> SettleBetAsync(BetData data);

        Task<BetResult> CancelTransactionAsync(BetData data);
    }

    public sealed class GamePlayActions : IGamePlayActions
    {
        private readonly IGameProviderCurrencyCodeMapper _currencyMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;

        public GamePlayActions(
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IGameProviderCurrencyCodeMapper currencyMapper)
        {
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _currencyMapper = currencyMapper;
        }

        Task<BetResult> IGamePlayActions.CancelTransactionAsync(BetData data)
        {
            throw new NotImplementedException();
        }

        async Task<GetBalanceResult> IGamePlayActions.GetBalanceAsync(string gameProviderCode, string playerCode)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(playerCode);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(gameProviderCode);

            var cur = await _currencyMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);

            var balance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false);

            return new GetBalanceResult(balance, cur);
        }

        Task<BetResult> IGamePlayActions.LockAmountAsync(BetData data)
        {
            throw new NotImplementedException();
        }

        Task<BetResult> IGamePlayActions.PlaceBetAsync(BetData data)
        {
            throw new NotImplementedException();
        }

        Task<BetResult> IGamePlayActions.SettleBetAsync(BetData data)
        {
            throw new NotImplementedException();
        }

        async Task<PlayerValidationResult> IGamePlayActions.ValidatePlayerAsync(PlayerValidationData data)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode);
            var status = ValidStatuses.GetStatus(player.Type);

            return new PlayerValidationResult();
        }

        #region Private Method
        

        #endregion
    }
}