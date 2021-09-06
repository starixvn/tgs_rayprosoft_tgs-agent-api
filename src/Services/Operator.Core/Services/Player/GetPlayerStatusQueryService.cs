using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Player
{
    internal class GetPlayerStatusQueryService : IGetService<GetPlayerStatusQuery, PlayerStatus>
    {
        private readonly ITgsRepository _tgsRepository;

        public GetPlayerStatusQueryService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<PlayerStatus> GetAsync(
            GetPlayerStatusQuery query,
            CancellationToken cancellationToken = default)
        {
            var player = await _tgsRepository.Players
                .SingleAsync(x => x.PlayerCode == query.PlayerCode)
                .ConfigureAwait(false);

            return player.Status;
        }
    }
}