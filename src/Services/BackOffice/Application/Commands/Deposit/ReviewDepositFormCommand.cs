using System;
using TGS.Services.BackOffice.Domain.Enums;

namespace TGS.Services.BackOffice.Application.Commands.Deposit
{
    public record ReviewDepositFormCommand(
        Guid FormId,
        ReviewStatus Status);
}
