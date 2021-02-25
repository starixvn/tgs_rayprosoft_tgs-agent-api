using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.Habanero.Services;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.Habanero.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Handlers
{
    public sealed class AuthorizePlayerHandler  : IRequestHandler<AuthorizePlayerRequest, AuthorizePlayerResponse>
    {
        private readonly HttpContext _httpContext;
        private readonly IGamePlayActions _actions;
        private readonly IHabaneroGameControllerMapper _map;

        private static readonly string GameProviderCode = Consts.GameProvider.Habanero;

        public AuthorizePlayerHandler(
            IHttpContextAccessor httpContextAccessor,
            IGamePlayActions actions,
            IHabaneroGameControllerMapper map)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _actions = actions;
            _map = map;
        }

        async Task<AuthorizePlayerResponse> IRequestHandler<AuthorizePlayerRequest, AuthorizePlayerResponse>.Handle(AuthorizePlayerRequest request, CancellationToken cancellationToken)
        {
            var playerCode = _httpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);

            if (request.Detail.GameLaunch)
            {
                var platformType = _httpContext.Items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

                var validationResult = await _actions.ValidatePlayerAsync(_map.Request(playerCode, platformType));
                return _map.Response(playerCode, validationResult);
            }
            
            var balanceResult = await _actions.GetBalanceAsync(GameProviderCode, playerCode);
            return _map.Response(playerCode, balanceResult);
        }
    }
}
