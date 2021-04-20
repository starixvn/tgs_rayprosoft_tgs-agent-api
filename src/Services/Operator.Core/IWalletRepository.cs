using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IWalletRepository
    {
        ValueTask<Guid> CreateDepositFormAsync(BankDepositForm form);

        ValueTask<BalanceInfo> GetBalanceInfoAsync(string playerCode);

        ValueTask<BankDepositForm> GetDepositFormByIdAsync(Guid id);

        IAsyncEnumerable<PaymentMethodInfo> GetAllPaymentMethodsAsync();

        IAsyncEnumerable<PaymentChannelInfo> GetAllPaymentChannelsAsync();

        ValueTask<BrandPaymentInfo> GetBrandPaymentByIdAsync(Guid id);

        ValueTask SubmitRemittanceReceiptToDepositFormAsync(Guid formId, string remittanceReceiptFileId);

        ValueTask<PaymentInfo> GetPaymentAsync(Guid id);

        IAsyncEnumerable<BrandPaymentInfo> GetBrandPaymentsAsync(Guid brandId);

        IAsyncEnumerable<BrandPaymentAcl> GetBrandPaymentAclsAsync(IEnumerable<Guid> brandPaymentIds, string playerCode);

        IAsyncEnumerable<BrandPaymentVip> GetBrandPaymentVipListAsync(IEnumerable<Guid> brandPaymentIds);

        ValueTask<PlayerInfo> GetPlayerInfoAsync(string playerCode);
    }
}
