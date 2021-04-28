using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Json;
using TGS.Services.Payment;
using TGS.ServiceUtility;
using TGS.Services.Operator.Deposit;
using Tgs.Payment;

namespace TGS.Services.Operator.Deposit
{
    internal class CreateDepositFormCommandService : ICommandService<CreateDepositFormCommand>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentClient _paymentClient;

        public CreateDepositFormCommandService(
            IPaymentRepository paymentRepository,
            IPaymentClient paymentClient)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
        }

        public async ValueTask ExecuteAsync(CreateDepositFormCommand command, CancellationToken cancellationToken = default)
        {
            var createFormId = Guid.NewGuid();

            var settings = command.CreatePaymentSettings(createFormId);
            var applyInfo = await _paymentClient
                .DepositAsync("", command.Amount, settings)
                .ConfigureAwait(false);

            var form = new DepositFormInfo(
                command.PlayerCode,
                command.BrandId,
                command.BrandPaymentId,
                command.Amount,
                applyInfo);

            await _paymentRepository
               .CreateDepositFormAsync(createFormId, form)
               .ConfigureAwait(false);

            command.ReturnFormIdSource.SetResult(createFormId);

        }
    }
}
