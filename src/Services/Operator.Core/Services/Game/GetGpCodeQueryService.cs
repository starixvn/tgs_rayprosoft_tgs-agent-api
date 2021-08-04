using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.Services.Operator.GameProviders;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Game
{
    internal class GetGpCodeQueryService : IGetService<GetGpCodeQuery, string>
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameProviderRepository _gameProviderRepository;

        public GetGpCodeQueryService(
            IGameProviderRepository gameProviderRepository,
            IGameProviderOperations gameProviderOperations)
        {
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }

        public async ValueTask<string> GetAsync(GetGpCodeQuery query, CancellationToken cancellationToken = default)
        {
            var subGp = await _gameProviderRepository
                .GetSubGameProviderDataInfoAsync(query.SubGpCode)
                .ConfigureAwait(false);

            if (subGp == null)
                throw new ArgumentException(
                    $"No game provider of code '{query.SubGpCode}' is found");

            return subGp.GpCode;
        }
    }
}
