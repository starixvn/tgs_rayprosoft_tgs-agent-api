using System;
using Tgs.Payment;

namespace TGS.Services.BackOffice.Application.Commands.Withdraw
{
    public record ReviewWithdrawFormCommand(
        Guid FormId,
        Guid? BrandPaymentId,
        ReviewState Status,
        string Remark);
}
