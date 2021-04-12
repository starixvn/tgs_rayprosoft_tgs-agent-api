using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TGS.Core.DataServices.Tgs;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.PaymentMethod
{
    internal class PaymentMethodQueryService : IQueryService<PaymentMethodQuery, PaymentMethodInfo>
    {
        private readonly ITgsRepository _tgsRepository;

        public PaymentMethodQueryService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public IAsyncEnumerable<PaymentMethodInfo> QueryAsync(PaymentMethodQuery query, CancellationToken cancellationToken = default)
        {
            return from paymentMethod in _tgsRepository.PaymentMethods.AsAsyncEnumerable()
                   select new PaymentMethodInfo(
                       paymentMethod.Id,
                       paymentMethod.Code);
        }
    }
}
