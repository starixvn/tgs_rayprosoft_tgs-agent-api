using System;
using TGS.Services.Payment;

namespace TGS.Services.BackOffice.Domain
{
    public class LeePayProcessor : IWithdrawProcessor
    {
        public string Subject => "tgs.payment.leepay.withdraw";

        public string MakeApplyInfo(Guid formId, decimal amount, BrandPaymentInfo brandPayment)
        {
            return "{}";
        }
    }
}
