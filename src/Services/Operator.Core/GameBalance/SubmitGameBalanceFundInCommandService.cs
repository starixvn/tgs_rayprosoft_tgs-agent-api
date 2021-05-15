using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.GameBalance
{
    internal class SubmitGameBalanceFundInCommandService : ICommandService<SubmitGameBalanceFundInCommand>
    {
        private readonly IPluggalbeGameProviderApiClient _pluggalbe;
        private readonly IPluggableOperations _pluggableOperations;

        public SubmitGameBalanceFundInCommandService(IPluggalbeGameProviderApiClient pluggalbe, IPluggableOperations pluggableOperations)
        {
            _pluggalbe = pluggalbe ?? throw new ArgumentNullException(nameof(pluggalbe));
            _pluggableOperations = pluggableOperations ?? throw new ArgumentNullException(nameof(pluggableOperations));
        }

        public async ValueTask ExecuteAsync(SubmitGameBalanceFundInCommand command, CancellationToken cancellationToken = default)
        {
            var serviceUrl = await _pluggableOperations.GetEndpointUrlByGameProviderCodeAsync(command.GameProviderCode);

            await _pluggalbe.FundInAsync(serviceUrl, new GameProviderWalletFundRequest()
            {
                GameProviderCode = command.GameProviderCode,
                PlayerCode = command.PlayerCode,
                Amount = command.Amount
            })
            .ConfigureAwait(false);
        }
    }
}