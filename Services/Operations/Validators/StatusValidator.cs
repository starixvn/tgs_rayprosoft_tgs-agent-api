using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;

namespace TGS.Services.Operations.Validators
{
    public interface IStatusValidator
    {
        Task ThrowIfLicenseeStatusIsNotOneOfAsync(Guid licenseeId, params Status[] statuses);

        Task ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(Guid gameProviderId, Guid gameProviderConfigurationId, params Status[] statuses);

        Task ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(Guid gameProviderId, Guid gameId, string externalGameId, Status[] statuses, GameStatus[] gameStatuses);

        void ThrowIfPlayerDisabled(PlayerStatus playerStatus);

        void ThrowIfPlayerDisabledToPlaceBet(PlayerStatus status);
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

        async Task IStatusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(Guid licenseeId, params Status[] statuses)
        {
            EnsureStatus(statuses);

            var licenseeStatus = await _licenseeOperations.GetLicenseeStatusAsync(licenseeId);

            if (licenseeStatus == null)
            {
                throw new InvalidStatusException($"Licensee doesn't exist");
            }

            if (!statuses.Contains(Status.Maintenance) && licenseeStatus == Status.Maintenance)
            {
                throw new LicenseeUnderMaintenanceException();
            }

            if (!statuses.Contains(licenseeStatus.Value))
            {
                throw new InvalidStatusException($"Licensee doesn't exist or is not active");
            }
        }

        async Task IStatusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(Guid gameProviderId, Guid gameProviderConfigurationId, params Status[] statuses)
        {
            EnsureStatus(statuses);

            var gameProviderStatus = await _gameProviderOperations.GetGameProviderStatusAsync(gameProviderId);
            var configurationStatus = await _gameProviderOperations.GetGameProviderConfigurationStatusAsync(gameProviderId, gameProviderConfigurationId);

            if (gameProviderStatus == null || configurationStatus == null)
            {
                throw new InvalidStatusException("GameProvider or GameProviderConfiguration doesn't exist");
            }

            if (!statuses.Contains(Status.Maintenance) && gameProviderStatus == Status.Maintenance || configurationStatus == Status.Maintenance)
            {
                throw new GameProviderUnderMaintenanceException();
            }

            if (!statuses.Contains(gameProviderStatus.Value) || !statuses.Contains(configurationStatus.Value))
            {
                throw new InvalidStatusException($"GameProvider or GameProviderConfiguration doesn't exist or is not active");
            }
        }

        async Task IStatusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(Guid gameProviderId, Guid gameId, string externalGameId, Status[] statuses, GameStatus[] gameStatuses)
        {
            EnsureStatus(statuses);
            EnsureStatus(gameStatuses);

            var gameProviderStatus = await _gameProviderOperations.GetGameProviderStatusAsync(gameProviderId);
            var gameStatus = await _gameProviderOperations.GetGameStatusAsync(gameProviderId, gameId);

            if (gameProviderStatus == null || gameStatus == null)
            {
                throw new InvalidStatusException("GameProvider or Game doesn't exist");
            }

            if (!statuses.Contains(Status.Maintenance) && gameProviderStatus == Status.Maintenance)
            {
                throw new GameProviderUnderMaintenanceException();
            }

            if (!statuses.Contains(gameProviderStatus.Value))
            {
                throw new InvalidStatusException($"GameProvider doesn't exist or is not active");
            }

            if (!gameStatuses.Contains(GameStatus.Maintenance) && gameStatus == GameStatus.Maintenance)
            {
                throw new GameUnderMaintenanceException();
            }

            if (!gameStatuses.Contains(gameStatus.Value))
            {
                var idToThrow = externalGameId.IsNullOrEmpty() ? gameId.ToString() : externalGameId;
                throw new InvalidStatusException($"Game '{idToThrow}' doesn't exist or is not active");
            }
        }

        void IStatusValidator.ThrowIfPlayerDisabled(PlayerStatus status)
        {
            EnsurePlayerStatus(status, PlayerStatus.Disabled);
        }

        void IStatusValidator.ThrowIfPlayerDisabledToPlaceBet(PlayerStatus status)
        {
            EnsurePlayerStatus(status, PlayerStatus.InBlacklist);
        }

        #region Private Methods

        private static void EnsureStatus<T>(T[] status) where T : struct
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