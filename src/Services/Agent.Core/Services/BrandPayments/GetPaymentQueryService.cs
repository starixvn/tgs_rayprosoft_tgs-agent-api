using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.BrandPayments
{
    internal class GetPaymentQueryService : IGetService<GetPaymentQuery, PaymentInfo>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentQueryService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public ValueTask<PaymentInfo> GetAsync(GetPaymentQuery query, CancellationToken cancellationToken = default)
            => _paymentRepository.GetPaymentAsync(query.PaymentId);
    }
}
