using System;
using TGS.Services.Payment;

namespace TGS.Services.BackOffice.Application.Commands.Deposit
{
    public record ReviewDepositFormCommand(
        Guid FormId,
        ReviewStatus Status,
        string remark);
}
