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
        ValueTask<PlayerValidationResult> ValidatePlayerAsync(PlayerValidationData data);
    }

    public sealed partial class GamePlayActions
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderCurrencyCodeMapper _currencyMapper;
        private readonly IExpirationValidator _expirationValidator;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameActionDataOperations _operations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IStatusValidator _statusValidator;
        private readonly IValidStatus<GameStatus> _validGameStatus;
        private readonly IValidStatus<Status> _validStatus;

        public GamePlayActions(
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
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
        async ValueTask<PlayerValidationResult> IGamePlayActions.ValidatePlayerAsync(PlayerValidationData data)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode).ConfigureAwait(false);
            var brandId = player.BrandId;

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(data.GameProviderCode)
                .ConfigureAwait(false);

            var gameProviderConfigurationId =
                await _brandOperations.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId)
                    .ConfigureAwait(false);

            await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(player.PlayerCode, data.PlatformType)
                .ConfigureAwait(false);

            await ValidateRequiredStatusAsync(player, gameProviderId, gameProviderConfigurationId)
                .ConfigureAwait(false);

            var cur = await _currencyMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId)
                .ConfigureAwait(false);

            var brand = await _brandOperations.GetCachedBrandAsync(brandId).ConfigureAwait(true);

            return new()
            {
                Balance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false),
                GameProviderCurrencyCode = cur,
                Username = player.Username,
                IsTestPlayer = player.IsTestPlayer,
                BrandCode = brand.Code,
                LicenseeCode = brand.LicenseeCode,
                ResellerCode = brand.ResellerCode
            };
        }

        /// <summary>
        /// Validate statuses on betting if needed.
        /// </summary>
        private async ValueTask ValidateRequiredStatusAsync(
            CachedPlayer player,
            Guid gameProviderId,
            Guid gameProviderConfigurationId)
        {
            var brandId = player.BrandId;
            var status = _validStatus.GetStatus(player.Type);

            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, status).ConfigureAwait(false);

            await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(gameProviderId,
                gameProviderConfigurationId,
                status).ConfigureAwait(false);

            await ValidatePlayerStatusAsync(player, gameProviderId).ConfigureAwait(false);
        }

        private async ValueTask ValidatePlayerStatusAsync(CachedPlayer player, Guid gameProviderId)
        {
            _statusValidator.ThrowIfPlayerDisabled(player.Status);

            if (!player.IsTestPlayer)
            {
                return;
            }

            var gameProviderSupportTestPlayer =
                await _gameProviderOperations.GetGameProviderSupportTestPlayerAsync(gameProviderId)
                    .ConfigureAwait(false);

            if (!gameProviderSupportTestPlayer)
            {
                throw new InvalidOperationException("Test player is not supported.");
            }
        }
    }
}