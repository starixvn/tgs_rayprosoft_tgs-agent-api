using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.BrandPayment
{
    internal class AllPaymentMethodChannelQueryService : IQueryService<AllPaymentMethodChannelQuery, PaymentMethodChannelInfo>
    {
        private readonly IPaymentRepository _paymentRepository;

        public AllPaymentMethodChannelQueryService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public IAsyncEnumerable<PaymentMethodChannelInfo> QueryAsync(
            AllPaymentMethodChannelQuery query,
            CancellationToken cancellationToken = default)
            => _paymentRepository.GetPaymentMethodChannelsAsync();
    }
}
