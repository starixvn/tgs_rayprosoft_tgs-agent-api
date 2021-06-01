using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.Withdraw
{
    internal class GetWithdrawFormDetailQueryService : IGetService<GetWithdrawFormDetailQuery, WithdrawFormInfo>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetWithdrawFormDetailQueryService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public ValueTask<WithdrawFormInfo> GetAsync(GetWithdrawFormDetailQuery query, CancellationToken cancellationToken = default)
            => _paymentRepository.GetWithdrawFormByIdAsync(query.FormId);
    }
}
