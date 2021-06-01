using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Deposit
{
    internal class GetBrandPaymentQueryService : IGetService<GetBrandPaymentQuery, BrandPaymentInfo>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetBrandPaymentQueryService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public ValueTask<BrandPaymentInfo> GetAsync(GetBrandPaymentQuery query, CancellationToken cancellationToken = default)
        {
            return _paymentRepository.GetBrandPaymentByIdAsync(query.Id);
        }
    }
}
