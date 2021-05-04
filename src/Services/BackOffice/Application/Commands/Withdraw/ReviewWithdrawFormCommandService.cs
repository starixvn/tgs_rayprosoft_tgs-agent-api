using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Commands.Withdraw
{
    internal class ReviewWithdrawFormCommandService : ICommandService<ReviewWithdrawFormCommand>
    {
        private readonly IPaymentRepository _paymentRepository;

        public ReviewWithdrawFormCommandService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public ValueTask ExecuteAsync(ReviewWithdrawFormCommand command, CancellationToken cancellationToken = default)
            => _paymentRepository.SignOffWithdrawFormAsync(command.FormId, command.Status, command.Remark);
    }
}
