using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Json;
using TGS.Services.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.BankDeposit
{
    internal class CreateBankDepositFormCommandService : ICommandService<CreateBankDepositFormCommand>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public CreateBankDepositFormCommandService(
            IPaymentRepository paymentRepository,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        public async ValueTask ExecuteAsync(CreateBankDepositFormCommand command, CancellationToken cancellationToken = default)
        {
            var createFormId = Guid.NewGuid();
            var form = new DepositFormInfo(
                command.PlayerCode,
                command.BrandId,
                command.BrandPaymentId,
                command.Amount,
                command.ApplyInfo);

            await _paymentRepository
               .CreateDepositFormAsync(createFormId, form)
               .ConfigureAwait(false);

            command.ReturnFormIdSource.SetResult(createFormId);
        }
    }
}
