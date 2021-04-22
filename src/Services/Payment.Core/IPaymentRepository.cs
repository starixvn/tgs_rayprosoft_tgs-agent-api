using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Services.Payment
{
    public interface IPaymentRepository
    {
        ValueTask CreateDepositFormAsync(Guid createFormId, DepositFormInfo form);

        ValueTask<DepositFormInfo> GetDepositFormByIdAsync(Guid id);

        IAsyncEnumerable<PaymentMethodInfo> GetAllPaymentMethodsAsync();

        IAsyncEnumerable<PaymentChannelInfo> GetAllPaymentChannelsAsync();

        ValueTask<BrandPaymentInfo> GetBrandPaymentByIdAsync(Guid id);

        ValueTask SubmitFormStatusAsync(Guid formId, int status, string paymentReturn);

        ValueTask<PaymentInfo> GetPaymentAsync(Guid id);

        IAsyncEnumerable<BrandPaymentInfo> GetBrandPaymentsAsync(Guid brandId);

        IAsyncEnumerable<BrandPaymentAcl> GetBrandPaymentAclsAsync(IEnumerable<Guid> brandPaymentIds, string playerCode);

        IAsyncEnumerable<BrandPaymentVip> GetBrandPaymentVipListAsync(IEnumerable<Guid> brandPaymentIds);
    }
}
