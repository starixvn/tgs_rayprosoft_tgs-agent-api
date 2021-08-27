using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using Tgs.Payment;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;
using TGS.Services.BackOffice.Application.Commands.Deposit;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Services
{
    public class AutoApproveDepositProcessor : IMessageHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AutoApproveDepositProcessor> _logger;

        public AutoApproveDepositProcessor(IServiceProvider serviceProvider, ILogger<AutoApproveDepositProcessor> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Receive AutoApproveDeposit request.");

            var request = ApproveDepositRequest.Parser.ParseFrom(data);

            var formId = Guid.Parse(request.FormId);
            var brandId = Guid.Parse(request.BrandId);

            using var scope = _serviceProvider.CreateScope();

            var reviewCommandService = scope.ServiceProvider.GetRequiredService<ICommandService<ReviewDepositFormCommand>>();

            var command = new ReviewDepositFormCommand(
                brandId,
                formId,
                ReviewState.Approve,
                null);

            await reviewCommandService
                .ExecuteAsync(command, cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation("AutoApproveDeposit processing completed.");
        }
    }
}
