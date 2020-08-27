using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Infrastructures.Redis;

namespace TGS.Services.Operations
{
    public interface IStatusValidator
    {
        Task ThrowIfBrandStatusIsNotOneOfAsync(Guid brandId, params Status[] status);

        Task ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(Guid gameProviderConfigurationId, params Status[] status);
    }

    public sealed class StatusValidator : IStatusValidator
    {
        private readonly IBrandOperations _brandOperations;

        public StatusValidator(IBrandOperations brandOperations)
        {
            _brandOperations = brandOperations;
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

        Task IStatusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(Guid gameProviderConfigurationId, params Status[] status)
        {
            throw new NotImplementedException();
        }

        private static void EnsureStatus(Status[] status)
        {
            if (status == null || status.Length == 0)
                throw new ArgumentException("No status provided");
        }
    }
}