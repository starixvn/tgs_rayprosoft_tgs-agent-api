using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment;
using Tgs.Payment.Clients;
using TGS.Services.BackOffice.Domain;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.WalletService;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Commands.Withdraw
{
    internal class ReviewWithdrawFormCommandService : ICommandService<ReviewWithdrawFormCommand>
    {
        private readonly IPaymentClient _paymentClient;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IWalletExchangeService _walletExchangeService;
        private readonly IWithdrawProcessorFactory _withdrawProcessorFactory;

        public ReviewWithdrawFormCommandService(
            IPaymentRepository paymentRepository,
            IWithdrawProcessorFactory withdrawProcessorFactory,
            IPaymentClient paymentClient,
            IWalletExchangeService walletExchangeService)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _withdrawProcessorFactory = withdrawProcessorFactory ?? throw new ArgumentNullException(nameof(withdrawProcessorFactory));
            _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
            _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
        }

        public async ValueTask ExecuteAsync(ReviewWithdrawFormCommand command, CancellationToken cancellationToken = default)
        {
            var formData = await _paymentRepository
                .GetWithdrawFormByIdAsync(command.FormId)
                .ConfigureAwait(false);

            if (formData.Status != 1000)
                return;

            if (command.Status == ReviewState.Approve)
            {
                var brandPayment = await _paymentRepository
                .GetBrandPaymentByIdAsync(command.BrandPaymentId.Value)
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
                        command.BrandPaymentId.Value,
                        applyInfo,
                        command.Status,
                        command.Remark)
                    .ConfigureAwait(false);
            }
            else
            {
                await _paymentRepository
                    .SignOffWithdrawFormAsync(
                        command.FormId,
                        null,
                        null,
                        command.Status,
                        command.Remark)
                    .ConfigureAwait(false);

                await _walletExchangeService.ExchangeAsync(
                    formData.BrandId,
                    formData.UserType,
                    formData.UserId,
                    TransactionTypes.CancelWithdrawal,
                    formData.Amount).ConfigureAwait(false);
            }
        }
    }
}