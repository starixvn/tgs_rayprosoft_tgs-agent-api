using System;

namespace TGS.Services.Payment
{
    public interface IWithdrawProcessor
    {
        string Subject { get; }

        string? MakeApplyInfo(Guid formId, decimal amount, BrandPaymentInfo brandPayment);
    }
}
