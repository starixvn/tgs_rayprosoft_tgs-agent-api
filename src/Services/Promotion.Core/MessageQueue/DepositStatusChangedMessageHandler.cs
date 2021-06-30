using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.MessageQueue;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;

namespace TGS.Services.Promotion.MessageQueue
{
    internal class DepositStatusChangedMessageHandler : IMessageHandler
    {
        private readonly IPaymentServiceClient _paymentServiceClient;

        public DepositStatusChangedMessageHandler(IPaymentServiceClient paymentServiceClient)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var formEvent = FormEvent.Parser.ParseFrom(data);

            if (formEvent == null)
                return;

            var depositForm = await _paymentServiceClient
                .GetDepositFormAsync(Guid.Parse(formEvent.BrandId), Guid.Parse(formEvent.FormId))
                .ConfigureAwait(false);
        }
    }
}
