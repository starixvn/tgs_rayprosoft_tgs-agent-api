using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;

namespace TGS.Services.Operations
{
    public interface IStatusValidator
    {
        Task ThrowIfBrandStatusIsNotOneOfAsync(Guid brandId, params Status[] status);

        Task ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(Guid gameProviderId, Guid gameProviderConfigurationId, params Status[] status);

        void ThrowIfPlayerDisabled(PlayerStatus playerStatus);
    }

    public sealed class StatusValidator : IStatusValidator
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;

        public StatusValidator(
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations)
        {
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
        }

        async Task IStatusValidator.ThrowIfBrandStatusIsNotOneOfAsync(Guid brandId, params Status[] status)
        {
            EnsureStatus(status);

            var brandStatus = await _brandOperations.GetBrandStatusByBrandIdAsync(brandId);

            if (brandStatus == null)
            {
                throw new InvalidStatusException($"Brand doesn't exist");
            }

            if (!status.Contains(Status.Maintenance) && brandStatus == Status.Maintenance)
            {
                throw new BrandUnderMaintenanceException();
            }

            if (!status.Contains(brandStatus.Value))
            {
                throw new InvalidStatusException($"Brand doesn't exist or is not active");
            }
        }

        Task IStatusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(Guid gameProviderId, Guid gameProviderConfigurationId, params Status[] status)
        {
            EnsureStatus(status);

            return Task.CompletedTask;
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