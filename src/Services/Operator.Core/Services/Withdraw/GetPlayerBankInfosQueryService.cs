using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Withdraw
{
    internal class GetPlayerBankInfosQueryService : IQueryService<GetPlayerBankInfosQuery, BankInfo>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPlayerBankInfosQueryService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public IAsyncEnumerable<BankInfo> QueryAsync(GetPlayerBankInfosQuery query, CancellationToken cancellationToken = default)
            => _paymentRepository.GetBankInfosByPlayerCodeAsync(query.PlayerCode);
    }
}
