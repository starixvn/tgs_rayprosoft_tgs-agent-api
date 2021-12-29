using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.BrandPayments
{
    internal class BrandPaymentsQueryService : IQueryService<BrandPaymentsQuery, BrandPaymentInfo>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;

        public BrandPaymentsQueryService(IPaymentServiceClient paymentServiceClient)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public IAsyncEnumerable<BrandPaymentInfo> QueryAsync(
            BrandPaymentsQuery query,
            CancellationToken cancellationToken = default)
            => _paymentServiceClient.GetBrandPaymentsAsync(query.BrandId);
    }
}
