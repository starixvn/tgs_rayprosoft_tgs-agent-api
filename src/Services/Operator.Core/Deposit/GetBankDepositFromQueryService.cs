using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Payment;
using TGS.ServiceUtility;
using TGS.Services.Operator.Deposit;

namespace TGS.Services.Operator.Deposit
{
    internal class GetBankDepositFromQueryService : IGetService<GetBankDepositFormQuery, DepositFormDetail>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetBankDepositFromQueryService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public async ValueTask<DepositFormDetail> GetAsync(GetBankDepositFormQuery query, CancellationToken cancellationToken = default)
        {
            var form = await _paymentRepository
                .GetDepositFormByIdAsync(query.FormId)
                .ConfigureAwait(false);

            if (form.PlayerCode != query.PlayerCode)
                throw new UltraViresException();

            return form;
        }
    }
}
