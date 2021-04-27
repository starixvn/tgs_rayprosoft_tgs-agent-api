using System;
using System.Collections.Generic;
using System.Threading;
using TGS.Services.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.PaymentMethod
{
    internal class PaymentMethodQueryService : IQueryService<PaymentMethodQuery, PaymentMethodInfo>
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentMethodQueryService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public IAsyncEnumerable<PaymentMethodInfo> QueryAsync(PaymentMethodQuery query,
            CancellationToken cancellationToken = default)
            => _paymentRepository.GetAllPaymentMethodsAsync();
    }
}
