using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IDepositRepository
    {
        ValueTask CreateDepositFormAsync(BankDepositForm form);

        ValueTask<BankDepositForm> GetDepositFormByIdAsync(Guid id);

        IAsyncEnumerable<PaymentMethod> GetAllPaymentMethodsAsync();

        IAsyncEnumerable<PaymentChannel> GetAllPaymentChannelsAsync();

        ValueTask<BrandPayment> GetBrandPaymentByIdAsync(Guid id);
    }
}
