using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;
using Tgs.ServiceUtility;
using TGS.Services.Operator.Services.GameBalance;
using Tgs.GameProviders.Pluggable.Client;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator.Services.GameBalance
{
    internal class GetGameBalanceQueryService : IGetService<GetGameBalanceQuery, decimal?>
    {
        private readonly IGameProvidersOperatorPluggableClient _providersOperatorPluggableClient;

        public GetGameBalanceQueryService(IGameProvidersOperatorPluggableClient providersOperatorPluggableClient)
        {
            _providersOperatorPluggableClient = providersOperatorPluggableClient ?? throw new ArgumentNullException(nameof(providersOperatorPluggableClient));
        }

        public ValueTask<decimal?> GetAsync(GetGameBalanceQuery query, CancellationToken cancellationToken = default)
            => _providersOperatorPluggableClient.GetBalanceAsync(
                query.PlayerCode,
                query.GameProviderCode);
    }
}
