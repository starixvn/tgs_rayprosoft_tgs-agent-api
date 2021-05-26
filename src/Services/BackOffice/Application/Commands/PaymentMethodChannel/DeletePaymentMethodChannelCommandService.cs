using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Commands.PaymentMethodChannel
{
    internal class DeletePaymentMethodChannelCommandService : ICommandService<DeletePaymentMethodChannelCommand>
    {
        private readonly ISystemPaymentRepository _systemPaymentRepository;

        public DeletePaymentMethodChannelCommandService(ISystemPaymentRepository systemPaymentRepository)
        {
            _systemPaymentRepository = systemPaymentRepository ?? throw new ArgumentNullException(nameof(systemPaymentRepository));
        }

        public ValueTask ExecuteAsync(DeletePaymentMethodChannelCommand command, CancellationToken cancellationToken = default)
            => _systemPaymentRepository.DeletePaymentMethodChannelAsync(command.Id);
    }
}
