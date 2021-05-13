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
    internal class GetGameBalanceQueryService : IGetService<GetGameBalanceQuery, decimal>
    {
        private readonly IPluggalbeGameProviderApiClient _client;

        private readonly ISettingOperations _settingOperations;

        public GetGameBalanceQueryService(IPluggalbeGameProviderApiClient client, ISettingOperations settingOperations)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
        }

        public async ValueTask<decimal> GetAsync(GetGameBalanceQuery query, CancellationToken cancellationToken = default)
        {
            var config = await _settingOperations.GetValueFromJsonByKeyAsync<PluggableConfigurationBase>(
                SettingConsts.GameProviderConfigurations.Template.Args(query.playercode));
            var url = config.ServiceEndpointUrl;

            var info = await _client.GetBalanceAsync(url, new GameProviderWalletGetBalanceRequest() { GameProviderCode = query.gpcode, PlayerCode = query.playercode });

            return info.Balance;
        }
    }
}