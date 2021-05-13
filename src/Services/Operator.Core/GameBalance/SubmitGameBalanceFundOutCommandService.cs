using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.GameBalance
{
    internal class SubmitGameBalanceFundOutCommandService : ICommandService<SubmitGameBalanceFundOutCommand>
    {
        private readonly IPluggalbeGameProviderApiClient _pluggalbe;
        private readonly ISettingOperations _settingOperations;

        public SubmitGameBalanceFundOutCommandService(IPluggalbeGameProviderApiClient pluggalbe, ISettingOperations settingOperations)
        {
            _pluggalbe = pluggalbe ?? throw new ArgumentNullException(nameof(pluggalbe));

            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
        }

        public async ValueTask ExecuteAsync(SubmitGameBalanceFundOutCommand command, CancellationToken cancellationToken = default)
        {
            var config = await _settingOperations.GetValueFromJsonByKeyAsync<PluggableConfigurationBase>(
                    SettingConsts.GameProviderConfigurations.Template.Args(command.PlayerCode));
            var url = config.ServiceEndpointUrl;

            await _pluggalbe.FundOutAsync(url, new GameProviderWalletFundRequest()
            {
                GameProviderCode = command.GameProviderCode,
                PlayerCode = command.PlayerCode,
                Amount = command.Amount
            })
            .ConfigureAwait(false);
        }
    }
}