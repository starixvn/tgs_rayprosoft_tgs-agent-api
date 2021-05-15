using System;

namespace TGS.Services.Payment
{
    public interface IWithdrawProcessor
    {
        string Subject { get; }

        string? MakeWithdrawSettings(
            Guid formId,
            decimal amount,
            string? bankInfo,
            BrandPaymentInfo brandPayment);
    }
}
