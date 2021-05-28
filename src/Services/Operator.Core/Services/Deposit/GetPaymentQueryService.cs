using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Payment;
using TGS.ServiceUtility;
using TGS.Services.Operator.Services.Deposit;

namespace TGS.Services.Operator.Services.Deposit
{
    internal class GetPaymentQueryService : IGetService<GetPaymentQuery, PaymentInfo>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentQueryService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public ValueTask<PaymentInfo> GetAsync(GetPaymentQuery query, CancellationToken cancellationToken = default)
        {
            return _paymentRepository.GetPaymentAsync(query.PaymentId);
        }
    }
}
