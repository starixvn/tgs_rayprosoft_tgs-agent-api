using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.BankDeposit
{
    internal class GetBankDepositFromQueryService : IGetService<GetBankDepositFormQuery, DepositFormInfo>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetBankDepositFromQueryService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public async ValueTask<DepositFormInfo> GetAsync(GetBankDepositFormQuery query, CancellationToken cancellationToken = default)
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
