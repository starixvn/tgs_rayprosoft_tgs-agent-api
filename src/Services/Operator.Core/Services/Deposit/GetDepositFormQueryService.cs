using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Payment;
using TGS.ServiceUtility;
using TGS.Services.Operator.Services.Deposit;

namespace TGS.Services.Operator.Services.Deposit
{
    internal class GetDepositFormQueryService : IGetService<GetDepositFormQuery, DepositFormDetail?>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetDepositFormQueryService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public async ValueTask<DepositFormDetail?> GetAsync(GetDepositFormQuery query, CancellationToken cancellationToken = default)
        {
            var formData = await _paymentRepository
                .GetDepositFormByIdAsync(query.FormId)
                .ConfigureAwait(false);

            if (formData.PlayerCode == query.PlayerCode)
                return formData;
            else
                return null;
        }
    }
}
