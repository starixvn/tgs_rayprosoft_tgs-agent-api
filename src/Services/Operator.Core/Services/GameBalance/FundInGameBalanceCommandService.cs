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
    internal class FundInGameBalanceCommandService : ICommandService<FundInGameBalanceCommand>
    {
        private readonly IGameProvidersOperatorPluggableClient _gameProvidersOperatorPluggableClient;
        private readonly ILogger<FundInGameBalanceCommandService> _logger;

        public FundInGameBalanceCommandService(
            IGameProvidersOperatorPluggableClient gameProvidersOperatorPluggableClient,
            ILogger<FundInGameBalanceCommandService> logger)
        {
            _gameProvidersOperatorPluggableClient = gameProvidersOperatorPluggableClient ?? throw new ArgumentNullException(nameof(gameProvidersOperatorPluggableClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask ExecuteAsync(FundInGameBalanceCommand command, CancellationToken cancellationToken = default)
        {


            var response = await _gameProvidersOperatorPluggableClient.GameProviderFundInAsync(
                command.PlayerCode,
                command.GameProviderCode,
                command.Amount).ConfigureAwait(false);

            if (response == null)
            {
                _logger.LogInformation("Failed to transfer the game wallet to the main wallet.");
                throw new GameBalanceFundInFailedException("Failed to transfer the game wallet to the main wallet.");
            }

            _logger.LogInformation("The game wallet was successfully transferred to the main wallet.");
        }
    }
}