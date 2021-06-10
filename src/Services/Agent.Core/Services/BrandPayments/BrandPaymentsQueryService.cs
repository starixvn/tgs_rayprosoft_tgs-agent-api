using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.BrandPayments
{
    internal class BrandPaymentsQueryService : IQueryService<BrandPaymentsQuery, BrandPaymentInfo>
    {
        private readonly IPaymentRepository _paymentRepository;

        public BrandPaymentsQueryService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public IAsyncEnumerable<BrandPaymentInfo> QueryAsync(
            BrandPaymentsQuery query,
            CancellationToken cancellationToken = default)
            => _paymentRepository.GetBrandPaymentsAsync(query.BrandId);
    }
}
