using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Application.Commands.Deposit;

namespace TGS.Apis.BackendOffice.Services
{
    public class AutoRejectDepositProcessor : IMessageHandler
    {
        private readonly ILogger<AutoRejectDepositProcessor> _logger;
        private readonly IServiceProvider _serviceProvider;

        public AutoRejectDepositProcessor(
            IServiceProvider serviceProvider,
            ILogger<AutoRejectDepositProcessor> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Receive AutoRejectDeposit request.");

            var request = RejectDepositRequest.Parser.ParseFrom(data);

            var formId = request.FormId.ToGuid();
            var brandId = request.BrandId.ToGuid();

            using var scope = _serviceProvider.CreateScope();

            var reviewCommandService =
                scope.ServiceProvider.GetRequiredService<ICommandService<ReviewDepositFormCommand>>();

            var command = new ReviewDepositFormCommand(
                brandId,
                formId,
                ReviewState.Reject,
                null,
                null,
                null,
                null);

            await reviewCommandService
                .ExecuteAsync(command, cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation("AutoRejectDeposit processing completed.");
        }
    }
}
