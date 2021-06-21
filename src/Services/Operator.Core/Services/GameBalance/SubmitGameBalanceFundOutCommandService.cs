using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.Core.Models.Enums;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.GameBalance
{
    internal class SubmitGameBalanceFundOutCommandService : ICommandService<SubmitGameBalanceFundOutCommand>
    {
        private readonly IPluggalbeGameProviderApiClient _pluggalbe;
        private readonly IPluggableOperations _pluggableOperations;
        private readonly ILogger<SubmitGameBalanceFundOutCommandService> _logger;

        public SubmitGameBalanceFundOutCommandService(IPluggalbeGameProviderApiClient pluggalbe, IPluggableOperations pluggableOperations, ILogger<SubmitGameBalanceFundOutCommandService> logger)
        {
            _pluggalbe = pluggalbe ?? throw new ArgumentNullException(nameof(pluggalbe));
            _pluggableOperations = pluggableOperations ?? throw new ArgumentNullException(nameof(pluggableOperations));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask ExecuteAsync(SubmitGameBalanceFundOutCommand command, CancellationToken cancellationToken = default)
        {
            var serviceUrl = await _pluggableOperations.GetEndpointUrlByGameProviderCodeAsync(command.GameProviderCode);

            _logger.LogInformation("Get Service Url From Pluggable Operations");

            var response = await _pluggalbe.FundOutAsync(serviceUrl, new GameProviderWalletFundRequest()
            {
                GameProviderCode = command.GameProviderCode,
                PlayerCode = command.PlayerCode,
                Amount = command.Amount
            })
            .ConfigureAwait(false);

            if (response.Error != (int)TgsApiError.None)
            {
                _logger.LogInformation("Pluggable Fund Out Has Error");
                throw new GameBalanceFundOutFailedException(response.ErrorMessage);
            }

            _logger.LogInformation("Pluggable Fund Out OK");
        }
    }
}