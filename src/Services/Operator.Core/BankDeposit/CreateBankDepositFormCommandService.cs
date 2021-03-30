using System;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.BankDeposit
{
    internal class CreateBankDepositFormCommandService : ICommandService<CreateBankDepositFormCommand>
    {
        private readonly IWalletRepository _walletRepository;

        public CreateBankDepositFormCommandService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask ExecuteAsync(CreateBankDepositFormCommand command)
        {
            var form = new BankDepositForm(
                command.PlayerCode,
                command.BrandId,
                command.BrandPaymentId,
                command.BankAccount,
                command.Amount);

            await _walletRepository
                .CreateDepositFormAsync(form)
                .ConfigureAwait(false);
        }
    }
}
