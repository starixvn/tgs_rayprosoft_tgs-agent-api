using System;
using System.Threading.Tasks;
using TGS.Core.Models;
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

        //Task<BetResult> LockAmountAsync(BetData data);

        Task<BetResult> PlaceBetAsync(BetData data);

        Task<BetResult> SettleBetAsync(BetData data);

        Task<BetResult> CancelTransactionAsync(BetData data);
    }

    public sealed class GamePlayActions : IGamePlayActions
    {
        private readonly IGameProviderCurrencyCodeMapper _currencyMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IBrandGameProviderOperations _brandGameProviderOpertaions;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly IStatusValidator _statusValidator;

        public GamePlayActions(
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOpertaions,
            IGameProviderCurrencyCodeMapper currencyMapper,
            IStatusValidator statusValidator)
        {
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandGameProviderOpertaions = brandGameProviderOpertaions;
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

        //Task<BetResult> IGamePlayActions.LockAmountAsync(BetData data)
        //{
        //    throw new NotImplementedException();
        //}

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
                        
            // TODO: check player login time?
            
            var brandId = player.BrandId;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(data.GameProviderCode);
            var gameProviderConfigurationId = await _brandOperations.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId);
            await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(gameProviderId, gameProviderConfigurationId, status);
            await ValidatePlayerStatus(player, gameProviderId);

            var cur = await _currencyMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);
            var lang = await _brandGameProviderOpertaions.MapCultureAsync(gameProviderId, brandId, player.CultureCode);
            var gameProviderBetLimitCode = await _gameProviderOperations.GetGameProviderBetLimitCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode, player.BetLimitGroupId);

            var cachedBrand = await _brandOperations.GetCachedBrandAsync(brandId);
                
            // TODO: check or get platform? how?

            return new PlayerValidationResult
            { 
                Balance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false),
                VipLevelId = gameProviderBetLimitCode,
                GameProviderCurrency = cur,
                Language = lang,
                Username = player.Username,
                IsTestPlayer = player.IsTestPlayer,
                BrandId = player.BrandId,
                BrandCode = cachedBrand.Code,
                ResellerCode = cachedBrand.ResellerCode
            };
        }

        private async Task ValidatePlayerStatus(CachedPlayer player, Guid gameProviderId)
        {
            _statusValidator.ThrowIfPlayerDisabled(player.Status);

            if (!player.IsTestPlayer)
            {
                return;
            }
          
            var gameProviderSupportTestPlayer = await _gameProviderOperations.GetGameProviderSupportTestPlayerAsync(gameProviderId);
            if (!gameProviderSupportTestPlayer)
            {
                throw new InvalidOperationException("Test player not supported.");
            }
        }
    }
}