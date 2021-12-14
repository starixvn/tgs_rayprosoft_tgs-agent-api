using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.Core.Models.Enums;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;
using Tgs.ServiceUtility;
using Tgs.GameProviders.Pluggable.Client;

namespace TGS.Services.Operator.Services.GameBalance
{
    internal class FundOutGameBalanceCommandService : ICommandService<FundOutGameBalanceCommand>
    {
        private readonly IGameProvidersOperatorPluggableClient _gameProvidersOperatorPluggableClient;
        private readonly ILogger<FundOutGameBalanceCommandService> _logger;

        public FundOutGameBalanceCommandService(
            IGameProvidersOperatorPluggableClient gameProvidersOperatorPluggableClient,
            ILogger<FundOutGameBalanceCommandService> logger)
        {
            _gameProvidersOperatorPluggableClient = gameProvidersOperatorPluggableClient ?? throw new ArgumentNullException(nameof(gameProvidersOperatorPluggableClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask ExecuteAsync(FundOutGameBalanceCommand command, CancellationToken cancellationToken = default)
        {
            var response = await _gameProvidersOperatorPluggableClient.GameProviderFundOutAsync(
               command.PlayerCode,
               command.GameProviderCode,
               command.Amount).ConfigureAwait(false);

            if (response == null)
            {
                _logger.LogInformation("Failed to transfer the main wallet to the game manufacturer.");
                throw new GameBalanceFundOutFailedException("Failed to transfer the main wallet to the game manufacturer.");
            }

            _logger.LogInformation("The main wallet was successfully transferred to the game manufacturer.");
        }
    }
}