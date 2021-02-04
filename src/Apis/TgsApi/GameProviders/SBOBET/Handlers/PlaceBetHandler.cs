using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Handlers
{
    public sealed class PlaceBetHandler : IRequestHandler<PlaceBetRequest, PlaceBetResponse>
    {
        private readonly IGamePlayActions _actions;
        private readonly ISBOBETGameControllerMapper _map;
        private readonly ISBOBETServices _services;

        public PlaceBetHandler(
            IGamePlayActions actions,
            ISBOBETGameControllerMapper map,
            ISBOBETServices services)
        {
            _actions = actions;
            _map = map;
            _services = services;
        }

        async Task<PlaceBetResponse> IRequestHandler<PlaceBetRequest, PlaceBetResponse>.Handle(PlaceBetRequest request, CancellationToken cancellationToken)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync();

            var betData = _map.Request(data, request);
            var r = await _actions.PlaceBetAsync(betData);
            return _map.Response(request, r);
        }
    }
}
