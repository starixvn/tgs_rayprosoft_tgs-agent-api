using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.GameBalance
{
    internal class SubmitGameBalanceFundInCommandService : ICommandService<SubmitGameBalanceFundInCommand>
    {
        private readonly IPluggalbeGameProviderApiClient _pluggalbe;
        private readonly IPluggableOperations _pluggableOperations;
        private readonly ILogger<SubmitGameBalanceFundInCommandService> _logger;

        public SubmitGameBalanceFundInCommandService(IPluggalbeGameProviderApiClient pluggalbe, IPluggableOperations pluggableOperations, ILogger<SubmitGameBalanceFundInCommandService> logger)
        {
            _pluggalbe = pluggalbe ?? throw new ArgumentNullException(nameof(pluggalbe));
            _pluggableOperations = pluggableOperations ?? throw new ArgumentNullException(nameof(pluggableOperations));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask ExecuteAsync(SubmitGameBalanceFundInCommand command, CancellationToken cancellationToken = default)
        {
            var serviceUrl = await _pluggableOperations.GetEndpointUrlByGameProviderCodeAsync(command.GameProviderCode);

            _logger.LogInformation("Get Service Url From Pluggable Operations");

            var response = await _pluggalbe.FundInAsync(serviceUrl, new GameProviderWalletFundRequest()
            {
                GameProviderCode = command.GameProviderCode,
                PlayerCode = command.PlayerCode,
                Amount = command.Amount
            })
            .ConfigureAwait(false);

            if (response.Error != 0)
            {
                _logger.LogInformation("Pluggable Fund In Has Error");
                throw new Exception(response.ErrorMessage);
            }

            _logger.LogInformation("Pluggable Fund In OK");
        }
    }
}