using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.GameBalance
{
    internal class GetGameBalanceQueryService : IGetService<GetGameBalanceQuery, GameBalanceInfo>
    {
        private readonly IPluggalbeGameProviderApiClient _client;

        private readonly IPluggableOperations _pluggableOperations;

        public GetGameBalanceQueryService(IPluggalbeGameProviderApiClient client, IPluggableOperations pluggableOperations)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _pluggableOperations = pluggableOperations ?? throw new ArgumentNullException(nameof(pluggableOperations));
        }

        public async ValueTask<GameBalanceInfo> GetAsync(GetGameBalanceQuery query, CancellationToken cancellationToken = default)
        {
            var serviceUrl = await _pluggableOperations.GetEndpointUrlByGameProviderCodeAsync(query.GameProviderCode);

            var info = await _client.GetBalanceAsync(
                serviceUrl,
                new GameProviderWalletGetBalanceRequest()
                {
                    GameProviderCode = query.GameProviderCode,
                    PlayerCode = query.PlayerCode
                });

            return new GameBalanceInfo(info.Balance);
        }
    }
}