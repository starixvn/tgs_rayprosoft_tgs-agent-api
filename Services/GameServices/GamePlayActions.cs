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
        private readonly IBrandOperations _brandOperations;
        private readonly IStatusValidator _statusValidator;

        public GamePlayActions(
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IGameProviderCurrencyCodeMapper currencyMapper,
            IStatusValidator statusValidator)
        {
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _currencyMapper = currencyMapper;
            _statusValidator = statusValidator;
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

            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(player.BrandId, status);

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(data.GameProviderCode);
            var gameProviderConfigurationId = await _brandOperations.GetBrandGameProviderConfigurationIdAsync(player.BrandId, gameProviderId);
            await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(gameProviderId, gameProviderConfigurationId, status);

            _statusValidator.ThrowIfPlayerDisabled(player.Status);

            return new PlayerValidationResult();
        }
    }
}