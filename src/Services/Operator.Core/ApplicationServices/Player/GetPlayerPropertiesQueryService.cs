using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Player
{
    internal class GetPlayerPropertiesQueryService
        : IGetService<GetPlayerPropertiesQuery, IReadOnlyDictionary<string, string>>
    {
        private readonly IPlayerServiceClient _client;

        public GetPlayerPropertiesQueryService(
            IPlayerServiceClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public ValueTask<IReadOnlyDictionary<string, string>> GetAsync(GetPlayerPropertiesQuery query,
            CancellationToken cancellationToken = default)
            => _client.GetPlayerPropertiesAsync(query.PlayerCode, cancellationToken);
    }
}
