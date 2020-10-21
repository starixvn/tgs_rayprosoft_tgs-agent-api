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

    public sealed partial class GamePlayActions : IGamePlayActions
    {
        private readonly IGameProviderCurrencyCodeMapper _currencyMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ILicenseeGameProviderOperations _LicenseeGameProviderOpertaions;
        private readonly IPlayerOperations _playerOperations;
        private readonly ILicenseeOperations _licenseeOperations;
        private readonly IGameActionDataOperations _operations;
        private readonly IStatusValidator _statusValidator;
        private readonly IExpirationValidator _expirationValidator;
        private readonly IValidStatus<Status> _validStatus;
        private readonly IValidStatus<GameStatus> _validGameStatus;
        private readonly IGamePlayActions _interface;

        public GamePlayActions(
            IPlayerOperations playerOperations,
            ILicenseeOperations licenseeOperations,
            IGameProviderOperations gameProviderOperations,
            ILicenseeGameProviderOperations licenseeGameProviderOpertaions,
            IGameActionDataOperations operations,
            IGameProviderCurrencyCodeMapper currencyMapper,
            IStatusValidator statusValidator,
            IExpirationValidator expirationValidator,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus)
        {
            _playerOperations = playerOperations;
            _licenseeOperations = licenseeOperations;
            _gameProviderOperations = gameProviderOperations;
            _LicenseeGameProviderOpertaions = licenseeGameProviderOpertaions;
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
            var licenseeId = player.LicenseeId;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(data.GameProviderCode);
            var gameProviderConfigurationId = await _licenseeOperations.GetLicenseeGameProviderConfigurationIdAsync(licenseeId, gameProviderId);

            await ValidtePlayerAysnc(player, gameProviderId, gameProviderConfigurationId, data.PlatformType, false);

            var cur = await _currencyMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);
            var lang = await _LicenseeGameProviderOpertaions.MapCultureAsync(gameProviderId, licenseeId, player.CultureCode);
            var gameProviderBetLimitCode = await _gameProviderOperations.GetGameProviderBetLimitCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode, player.BetLimitGroupId);

            var cachedLicensee = await _licenseeOperations.GetCachedLicenseeAsync(licenseeId);

            return new PlayerValidationResult
            {
                Balance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false),
                VipLevelId = gameProviderBetLimitCode,
                GameProviderCurrency = cur,
                Language = lang,
                Username = player.Username,
                IsTestPlayer = player.IsTestPlayer,
                LicenseeId = licenseeId,
                LicenseeCode = cachedLicensee.Code,
                ResellerCode = cachedLicensee.ResellerCode
            };
        }

        /// <summary>
        /// Validate player on betting if needed.
        /// </summary>
        private async Task ValidtePlayerAysnc(CachedPlayer player, Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType? platform, bool ignoreExpired = false)
        {
            if (!ignoreExpired)
            {
                await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(player.PlayerCode, platform);
            }

            var licenseeId = player.LicenseeId;
            var status = _validStatus.GetStatus(player.Type);
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, status);
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