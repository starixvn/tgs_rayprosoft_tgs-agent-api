using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Commands.PaymentMethodChannel
{
    internal class CreatePaymentMethodChannelCommandService : ICommandService<CreatePaymentMethodChannelCommand>
    {
        private readonly ISystemPaymentRepository _systemPaymentRepository;

        public CreatePaymentMethodChannelCommandService(ISystemPaymentRepository systemPaymentRepository)
        {
            _systemPaymentRepository = systemPaymentRepository ?? throw new ArgumentNullException(nameof(systemPaymentRepository));
        }

        public ValueTask ExecuteAsync(CreatePaymentMethodChannelCommand command, CancellationToken cancellationToken = default)
            => _systemPaymentRepository.CreatePaymentMethodChannelAsync(command.PaymentId, command.MethodId, command.ChannelId);
    }
}
