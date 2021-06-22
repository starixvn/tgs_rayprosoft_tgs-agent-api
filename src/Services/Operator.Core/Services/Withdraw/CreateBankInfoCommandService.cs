using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment;
using Tgs.Payment.Clients;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Withdraw
{
    internal class CreateBankInfoCommandService : ICommandService<CreateBankInfoCommand>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;

        public CreateBankInfoCommandService(IPaymentServiceClient paymentServiceClient)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public async ValueTask ExecuteAsync(CreateBankInfoCommand command, CancellationToken cancellationToken = default)
        {
            var bankInfo = new BankCreateInfo
            {
                BankType = command.BankType,
                PaymentChannelId = command.PaymentChannelId,
                Detail = JsonDocument.Parse(command.Detail)
            };

            await _paymentServiceClient
                .CreatePlayerBankInfoAsync(command.BrandId, command.PlayerCode, bankInfo)
                .ConfigureAwait(false);
        }
    }
}
