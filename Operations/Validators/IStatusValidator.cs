using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Validators;

public interface IStatusValidator
{
    Task ThrowIfBrandStatusIsNotOneOfAsync(Guid brandId, params Status[] statuses);

    void ThrowIfPlayerDisabled(PlayerStatus playerStatus);
}