using System;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.BankDeposit
{
    internal class GetBankDepositFromQueryService : IGetService<GetBankDepositFormQuery, BankDepositForm>
    {
        private readonly IWalletRepository _walletRepository;

        public GetBankDepositFromQueryService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask<BankDepositForm> GetAsync(GetBankDepositFormQuery query)
        {
            var form = await _walletRepository
                .GetDepositFormByIdAsync(query.FormId)
                .ConfigureAwait(false);

            if (form.PlayerCode != query.PlayerCode)
                throw new UltraViresException();

            return form;
        }
    }
}
