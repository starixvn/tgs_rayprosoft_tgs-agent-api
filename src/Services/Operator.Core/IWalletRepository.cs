using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IWalletRepository
    {
        ValueTask CreateDepositFormAsync(BankDepositForm form);

        ValueTask<BalanceInfo> GetBalanceInfoAsync(string playerCode);

        ValueTask<BankDepositForm> GetDepositFormByIdAsync(Guid id);

        IAsyncEnumerable<PaymentMethod> GetAllPaymentMethodsAsync();

        IAsyncEnumerable<PaymentChannel> GetAllPaymentChannelsAsync();

        ValueTask<BrandPayment> GetBrandPaymentByIdAsync(Guid id);
    }
}
