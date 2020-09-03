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
        Task<PlayerValidationResult> ValidatePlayerAsync(PlayerValidationData data);

        Task ValidatePlayerForBettingAsync(PlayerValidationData data, bool ignoreLoginExpired = false);
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

        public GamePlayActions(
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOpertaions,
            IGameProviderCurrencyCodeMapper currencyMapper,
            IStatusValidator statusValidator,
            IExpirationValidator expirationValidator)
        {
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandGameProviderOpertaions = brandGameProviderOpertaions;
            _currencyMapper = currencyMapper;
            _statusValidator = statusValidator;
            _expirationValidator = expirationValidator;
        }

        async Task<PlayerValidationResult> IGamePlayActions.ValidatePlayerAsync(PlayerValidationData data)
        {
            var (player, gameProviderId) = await ValidtePlayerAysnc(data);

            var brandId = player.BrandId;
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

        Task IGamePlayActions.ValidatePlayerForBettingAsync(PlayerValidationData data, bool ignoreExpired)
        {
            return ValidtePlayerAysnc(data, ignoreExpired);
        }

        private async Task<(CachedPlayer player, Guid gameProviderId)> ValidtePlayerAysnc(PlayerValidationData data, bool ignoreExpired = false)
        {
            var playerCode = data.PlayerCode;
            var player = await _playerOperations.GetCachedPlayerAsync(playerCode);

            if (!ignoreExpired)
            {
                await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(playerCode);
            }

            var brandId = player.BrandId;
            var status = ValidStatuses.GetStatus(player.Type);
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, status);

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(data.GameProviderCode);
            var gameProviderConfigurationId = await _brandOperations.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId);
            await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(gameProviderId, gameProviderConfigurationId, status);
            await ValidatePlayerStatusAsync(player, gameProviderId);

            return (player, gameProviderId);
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