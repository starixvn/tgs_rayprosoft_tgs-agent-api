using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;
using TGS.ServiceUtility;
using TGS.Services.Operator.Services.GameBalance;

namespace TGS.Services.Operator.Services.GameBalance
{
    internal class SubmitGameBalanceFundOutCommandService : ICommandService<SubmitGameBalanceFundOutCommand>
    {
        private readonly IPluggalbeGameProviderApiClient _pluggalbe;
        private readonly IPluggableOperations _pluggableOperations;

        public SubmitGameBalanceFundOutCommandService(IPluggalbeGameProviderApiClient pluggalbe, IPluggableOperations pluggableOperations)
        {
            _pluggalbe = pluggalbe ?? throw new ArgumentNullException(nameof(pluggalbe));

            _pluggableOperations = pluggableOperations ?? throw new ArgumentNullException(nameof(pluggableOperations));
        }

        public async ValueTask ExecuteAsync(SubmitGameBalanceFundOutCommand command, CancellationToken cancellationToken = default)
        {
            var serviceUrl = await _pluggableOperations.GetEndpointUrlByGameProviderCodeAsync(command.GameProviderCode);

            await _pluggalbe.FundOutAsync(serviceUrl, new GameProviderWalletFundRequest()
            {
                GameProviderCode = command.GameProviderCode,
                PlayerCode = command.PlayerCode,
                Amount = command.Amount
            })
            .ConfigureAwait(false);
        }
    }
}
