using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;

namespace TGS.Services.Operations.Validators
{
    public interface IStatusValidator
    {
        Task ThrowIfLicenseeStatusIsNotOneOfAsync(Guid licenseeId, params Status[] status);

        Task ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(Guid gameProviderId, Guid gameProviderConfigurationId, params Status[] status);

        void ThrowIfPlayerDisabled(PlayerStatus playerStatus);
    }

    public sealed class StatusValidator : IStatusValidator
    {
        private readonly ILicenseeOperations _licenseeOperations;
        private readonly IGameProviderOperations _gameProviderOperations;

        public StatusValidator(
            ILicenseeOperations licenseeOperations,
            IGameProviderOperations gameProviderOperations)
        {
            _licenseeOperations = licenseeOperations;
            _gameProviderOperations = gameProviderOperations;
        }

        async Task IStatusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(Guid LicenseeId, params Status[] status)
        {
            EnsureStatus(status);

            var licenseeStatus = await _licenseeOperations.GetLicenseeStatusAsync(LicenseeId);

            if (licenseeStatus == null)
            {
                throw new InvalidStatusException($"Licensee doesn't exist");
            }

            if (!status.Contains(Status.Maintenance) && licenseeStatus == Status.Maintenance)
            {
                throw new LicenseeUnderMaintenanceException();
            }

            if (!status.Contains(licenseeStatus.Value))
            {
                throw new InvalidStatusException($"Licensee doesn't exist or is not active");
            }
        }

        async Task IStatusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(Guid gameProviderId, Guid gameProviderConfigurationId, params Status[] status)
        {
            EnsureStatus(status);

            var gameProviderStatus = await _gameProviderOperations.GetGameProviderStatusAsync(gameProviderId);
            var configurationStatus = await _gameProviderOperations.GetGameProviderConfigurationStatusAsync(gameProviderId, gameProviderConfigurationId);

            if (gameProviderStatus == null || configurationStatus == null)
            {
                throw new InvalidStatusException("GameProvider or GameProviderConfiguration doesn't exist");
            }

            if (!status.Contains(Status.Maintenance) && gameProviderStatus == Status.Maintenance || configurationStatus == Status.Maintenance)
            {
                throw new GameProviderUnderMaintenanceException();
            }

            if (!status.Contains(gameProviderStatus.Value) || !status.Contains(configurationStatus.Value))
            {
                throw new InvalidStatusException($"GameProvider or GameProviderConfiguration doesn't exist or is not active");
            }
        }

        void IStatusValidator.ThrowIfPlayerDisabled(PlayerStatus status)
        {
            EnsurePlayerStatus(status, PlayerStatus.Disabled);
        }

        #region Private Methods

        private static void EnsureStatus(Status[] status)
        {
            if (status == null || status.Length == 0)
                throw new ArgumentException("No status provided");
        }

        private void EnsurePlayerStatus(PlayerStatus playerStatus, PlayerStatus statusToThrowException)
        {
            if (playerStatus == PlayerStatus.Enabled)
                return;

            if ((playerStatus & statusToThrowException) == statusToThrowException)
            {
                throw new PlayerDisabledException(playerStatus);
            }
        }

        #endregion Private Methods
    }
}