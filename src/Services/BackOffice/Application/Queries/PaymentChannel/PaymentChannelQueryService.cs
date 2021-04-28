using System;
using System.Collections.Generic;
using System.Threading;
using TGS.Services.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.PaymentChannel
{
    internal class PaymentChannelQueryService : IQueryService<PaymentChannelQuery, PaymentChannelInfo>
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentChannelQueryService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public IAsyncEnumerable<PaymentChannelInfo> QueryAsync(
            PaymentChannelQuery query,
            CancellationToken cancellationToken = default)
            => _paymentRepository.GetAllPaymentChannelsAsync();
    }
}
