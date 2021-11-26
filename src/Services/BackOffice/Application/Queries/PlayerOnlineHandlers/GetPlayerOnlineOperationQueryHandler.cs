using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.PlayerOnline;
using TGS.Services.BackOffice.Domain;
using TGS.Core.DataServices.Base.Extensions;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.PlayerOnlineHandlers
{
    public class GetPlayerOnlineOperationQueryHandler : IGetService<GetPlayerOnlineRequest, PagedResponse<PlayerOnlineModel>>
    {
        private readonly IPlayerOnlineRepository _playerOnlineRepository;
        private readonly IPlayerService _playerService;

        public GetPlayerOnlineOperationQueryHandler(
            IPlayerOnlineRepository playerOnlineRepository,
            IPlayerService playerService)
        {

            _playerOnlineRepository = playerOnlineRepository ?? throw new ArgumentNullException(nameof(playerOnlineRepository));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask<PagedResponse<PlayerOnlineModel>> GetAsync(GetPlayerOnlineRequest query, CancellationToken cancellationToken = default)
        {
            var playerCodes = await _playerOnlineRepository
                 .GetQueryAsync(query.BrandId)
                 .ConfigureAwait(false);

            return await (_playerService.GetPlayerQueries(
                x => x.Where(x => x.BrandId == query.BrandId)
                .Where(x => playerCodes.Contains(x.PlayerCode))
                .Where(query.VipLevelId.HasValue, x => x.Vip.Id == query.VipLevelId.Value)
                .Where(query.Status.HasValue, x => x.Status == query.Status.Value)
                .Where(!string.IsNullOrEmpty(query.PlayerCode), x => x.PlayerCode == query.PlayerCode)))
                .Select(x => new PlayerOnlineModel()
                {
                    BrandId = x.BrandId,
                    PlayerCode = x.PlayerCode,
                    Username = x.Username,
                    RealName = x.RealName,
                    Status = x.Status,
                    VipLevelId = x.Vip.Id,
                    VipLevel = x.Vip.Level
                })
                .CreatePagedResponseAsync(query)
                .ConfigureAwait(false);
        }
    }
}
