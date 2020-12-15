using System;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations.Data;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Validators;

namespace TGS.Services.GameOperations
{
    public partial interface IGamePlayActions
    {
        Task<PlayerValidationResult> ValidatePlayerAsync(PlayerValidationData data);
    }

    public sealed partial class GamePlayActions
    {
        private readonly IGameProviderCurrencyCodeMapper _currencyMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameActionDataOperations _operations;
        private readonly IStatusValidator _statusValidator;
        private readonly IExpirationValidator _expirationValidator;
        private readonly IValidStatus<Status> _validStatus;
        private readonly IValidStatus<GameStatus> _validGameStatus;

        public GamePlayActions(
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IGameActionDataOperations operations,
            IGameProviderCurrencyCodeMapper currencyMapper,
            IStatusValidator statusValidator,
            IExpirationValidator expirationValidator,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus)
        {
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _currencyMapper = currencyMapper;
            _statusValidator = statusValidator;
            _expirationValidator = expirationValidator;
            _validStatus = validStatus;
            _validGameStatus = validGameStatus;
            _operations = operations;
        }

        /// <summary>
        /// Validate player for single game api
        /// </summary>
        async Task<PlayerValidationResult> IGamePlayActions.ValidatePlayerAsync(PlayerValidationData data)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode);
            var brandId = player.BrandId;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(data.GameProviderCode);
            var gameProviderConfigurationId = await _brandOperations.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId);

            await ValidatePlayerAsync(player, gameProviderId, gameProviderConfigurationId, data.PlatformType);

            var cur = await _currencyMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);
            var lang = await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId, player.CultureCode);
            
            var brand = await _brandOperations.GetCachedBrandAsync(brandId);

            return new()
            {
                Balance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false),
                GameProviderCurrency = cur,
                Language = lang,
                Username = player.Username,
                IsTestPlayer = player.IsTestPlayer,
                BrandCode = brand.Code,
                LicenseeCode = brand.LicenseeCode,
                ResellerCode = brand.ResellerCode
            };
        }

        /// <summary>
        /// Validate player on betting if needed.
        /// </summary>
        private async Task ValidatePlayerAsync(CachedPlayer player, Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType? platform, bool ignoreExpired = false)
        {
            if (!ignoreExpired)
            {
                await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(player.PlayerCode, platform);
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