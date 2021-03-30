using System;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.BankDeposit
{
    internal class GetBankDepositFromQueryService : IGetService<GetBankDepositFormQuery, BankDepositForm>
    {
        private readonly IDepositRepository _depositRepository;

        public GetBankDepositFromQueryService(IDepositRepository depositRepository)
        {
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
        }

        public async ValueTask<BankDepositForm> GetAsync(GetBankDepositFormQuery query)
        {
            var form = await _depositRepository
                .GetDepositFormByIdAsync(query.FormId)
                .ConfigureAwait(false);

            if (form.PlayerCode != query.PlayerCode)
                throw new UltraViresException();

            return form;
        }
    }
}
