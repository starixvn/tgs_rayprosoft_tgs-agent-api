using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;

namespace TGS.Services.Operations.Validators;

public sealed class StatusValidator(IBrandOperations brandOperations) : IStatusValidator
{
    public async Task ThrowIfBrandStatusIsNotOneOfAsync(Guid brandId, params Status[] statuses)
    {
        EnsureStatus(statuses);

        var status = await brandOperations.GetBrandStatusAsync(brandId);

        if (status == null)
        {
            throw new InvalidStatusException("Brand doesn't exist");
        }

        if (!statuses.Contains(Status.Maintenance) && status == Status.Maintenance)
        {
            throw new BrandUnderMaintenanceException("Brand is put into maintenance");
        }

        if (!statuses.Contains(status.Value))
        {
            throw new InvalidStatusException("Brand doesn't exist or is not active");
        }
    }

    public void ThrowIfPlayerDisabled(PlayerStatus status) =>
        EnsurePlayerStatus(status, PlayerStatus.Disabled);

    #region Private Methods

    private static void EnsurePlayerStatus(PlayerStatus playerStatus, PlayerStatus statusToThrowException)
    {
        if (playerStatus == PlayerStatus.Enabled)
        {
            return;
        }

        if (playerStatus == statusToThrowException)
        {
            throw new PlayerDisabledException(playerStatus);
        }
    }

    private static void EnsureStatus<T>(T[] status) where T : struct
    {
        if (status == null || status.Length == 0)
        {
            throw new ArgumentException("No status provided");
        }
    }

    #endregion Private Methods
}
