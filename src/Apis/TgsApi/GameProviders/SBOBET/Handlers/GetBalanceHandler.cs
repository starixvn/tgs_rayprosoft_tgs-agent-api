using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Models;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Handlers
{
    public sealed class GetBalanceHandler : IRequestHandler<GetBalanceRequest, GetBalanceResponse>
    {
        private readonly IGamePlayActions _actions;
        private readonly ISBOBETGameControllerMapper _map;

        public GetBalanceHandler(
            IGamePlayActions actions,
            ISBOBETGameControllerMapper map)
        {
            _actions = actions;
            _map = map;
        }

        async Task<GetBalanceResponse> IRequestHandler<GetBalanceRequest, GetBalanceResponse>.Handle(GetBalanceRequest request, CancellationToken cancellationToken)
        {
            var r = await _actions.GetBalanceAsync(Consts.GameProvider.SBOBET, request.PlayerCode);
            return _map.Response(request.PlayerCode, r);
        }
    }
}
