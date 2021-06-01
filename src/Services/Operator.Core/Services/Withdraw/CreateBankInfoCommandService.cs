using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Withdraw
{
    internal class CreateBankInfoCommandService : ICommandService<CreateBankInfoCommand>
    {
        private readonly IPaymentRepository _paymentRepository;

        public CreateBankInfoCommandService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public ValueTask ExecuteAsync(CreateBankInfoCommand command, CancellationToken cancellationToken = default)
        {
            var bankInfo = new BankInfo(
                Guid.NewGuid(),
                command.PlayerCode,
                command.BrandId,
                command.PaymentChannelId,
                command.BankType,
                command.Detail);

            return _paymentRepository.CreateBankInfoAsync(bankInfo, command.PlayerCode);
        }
    }
}
