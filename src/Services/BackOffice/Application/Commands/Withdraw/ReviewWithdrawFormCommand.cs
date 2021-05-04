using System;
using TGS.Services.Payment;

namespace TGS.Services.BackOffice.Application.Commands.Withdraw
{
    public record ReviewWithdrawFormCommand(
        Guid FormId,
        ReviewStatus Status,
        string Remark);
}
