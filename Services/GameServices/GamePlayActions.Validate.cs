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
    public partial interface IGamePlayActions
    {
        Task<PlayerValidationResult> ValidatePlayerAsync(PlayerValidationData data);
    }

    public sealed partial class GamePlayActions : IGamePlayActions
    {
        private readonly IGameProviderCurrencyCodeMapper _currencyMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IBrandGameProviderOperations _brandGameProviderOpertaions;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly IStatusValidator _statusValidator;
        private readonly IExpirationValidator _expirationValidator;
        private readonly IValidStatus<Status> _validStatus;
        private readonly IValidStatus<GameStatus> _validGameStatus;

        public GamePlayActions(
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOpertaions,
            IGameProviderCurrencyCodeMapper currencyMapper,
            IStatusValidator statusValidator,
            IExpirationValidator expirationValidator,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus)
        {
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandGameProviderOpertaions = brandGameProviderOpertaions;
            _currencyMapper = currencyMapper;
            _statusValidator = statusValidator;
            _expirationValidator = expirationValidator;
            _validStatus = validStatus;
            _validGameStatus = validGameStatus;
        }

        async Task<PlayerValidationResult> IGamePlayActions.ValidatePlayerAsync(PlayerValidationData data)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode);
            var brandId = player.BrandId;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(data.GameProviderCode);
            var gameProviderConfigurationId = await _brandOperations.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId);

            await ValidtePlayerAysnc(player, gameProviderId, gameProviderConfigurationId, false);
            
            var cur = await _currencyMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);
            var lang = await _brandGameProviderOpertaions.MapCultureAsync(gameProviderId, brandId, player.CultureCode);
            var gameProviderBetLimitCode = await _gameProviderOperations.GetGameProviderBetLimitCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode, player.BetLimitGroupId);

            var cachedBrand = await _brandOperations.GetCachedBrandAsync(brandId);

            return new PlayerValidationResult
            {
                Balance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false),
                VipLevelId = gameProviderBetLimitCode,
                GameProviderCurrency = cur,
                Language = lang,
                Username = player.Username,
                IsTestPlayer = player.IsTestPlayer,
                BrandId = brandId,
                BrandCode = cachedBrand.Code,
                ResellerCode = cachedBrand.ResellerCode
            };
        }

        private async Task ValidtePlayerAysnc(CachedPlayer player, Guid gameProviderId, Guid gameProviderConfigurationId, bool ignoreExpired = false)
        {            
            if (!ignoreExpired)
            {
                await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(player.PlayerCode);
            }

            var brandId = player.BrandId;
            var status = _validStatus.GetStatus(player.Type);
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, status);
            await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(gameProviderId, gameProviderConfigurationId, status);
            await ValidatePlayerStatusAsync(player, gameProviderId);
        }

        private async Task ValidatePlayerStatusAsync(CachedPlayer player, Guid gameProviderId)
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