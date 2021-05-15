using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment;
using TGS.Services.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Commands.Withdraw
{
    internal class ReviewWithdrawFormCommandService : ICommandService<ReviewWithdrawFormCommand>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IWithdrawProcessorFactory _withdrawProcessorFactory;
        private readonly IPaymentClient _paymentClient;

        public ReviewWithdrawFormCommandService(
            IPaymentRepository paymentRepository,
            IWithdrawProcessorFactory withdrawProcessorFactory,
            IPaymentClient paymentClient)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _withdrawProcessorFactory = withdrawProcessorFactory ?? throw new ArgumentNullException(nameof(withdrawProcessorFactory));
            _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
        }

        public async ValueTask ExecuteAsync(ReviewWithdrawFormCommand command, CancellationToken cancellationToken = default)
        {
            var formData = await _paymentRepository
                .GetWithdrawFormByIdAsync(command.FormId)
                .ConfigureAwait(false);

            var brandPayment = await _paymentRepository
                .GetBrandPaymentByIdAsync(command.BrandPaymentId)
                .ConfigureAwait(false);

            var payment = await _paymentRepository
                .GetPaymentAsync(brandPayment.PaymentId)
                .ConfigureAwait(false);

            var processor = _withdrawProcessorFactory.GetWithdrawProcessor(payment.Code);

            var withdrawSettings = processor.MakeWithdrawSettings(command.FormId, formData.Amount, formData.BankInfo, brandPayment);

            var applyInfo = await _paymentClient
                .WithdrawAsync(processor.Subject, formData.Amount, withdrawSettings)
                .ConfigureAwait(false);

            await _paymentRepository
                .SignOffWithdrawFormAsync(
                    command.FormId,
                    command.BrandPaymentId,
                    applyInfo,
                    command.Status,
                    command.Remark)
                .ConfigureAwait(false);
        }
    }
}
