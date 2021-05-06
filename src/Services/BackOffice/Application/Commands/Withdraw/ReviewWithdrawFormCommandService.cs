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
        private readonly IWithdrawProcessorFactory _withdrawProcessorFactory;

        public ReviewWithdrawFormCommandService(
            IPaymentRepository paymentRepository,
            IWithdrawProcessorFactory withdrawProcessorFactory)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _withdrawProcessorFactory = withdrawProcessorFactory ?? throw new ArgumentNullException(nameof(withdrawProcessorFactory));
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

            var applyInfo = processor.MakeApplyInfo(command.FormId, formData.Amount, brandPayment);

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
