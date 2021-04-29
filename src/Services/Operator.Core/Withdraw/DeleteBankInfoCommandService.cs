using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Withdraw
{
    internal class DeleteBankInfoCommandService : ICommandService<DeleteBankInfoCommand>
    {
        private readonly IPaymentRepository _paymentRepository;

        public DeleteBankInfoCommandService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public ValueTask ExecuteAsync(DeleteBankInfoCommand command, CancellationToken cancellationToken = default)
            => _paymentRepository.DeleteBankInfoAsync(command.PlayerCode, command.BankInfoId);
    }
}
