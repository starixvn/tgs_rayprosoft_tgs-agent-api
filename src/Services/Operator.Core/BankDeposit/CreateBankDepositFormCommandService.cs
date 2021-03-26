using System;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.BankDeposit
{
    internal class CreateBankDepositFormCommandService : ICommandService<CreateBankDepositFormCommand>
    {
        private readonly IDepositRepository _depositRepository;

        public CreateBankDepositFormCommandService(IDepositRepository depositRepository)
        {
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
        }

        public async ValueTask ExecuteAsync(CreateBankDepositFormCommand command)
        {
            var form = new BankDepositForm(
                command.PlayerCode,
                command.BrandId,
                command.BrandPaymentId,
                command.BankAccount,
                command.Amount);

            await _depositRepository
                .CreateDepositFormAsync(form)
                .ConfigureAwait(false);
        }
    }
}
