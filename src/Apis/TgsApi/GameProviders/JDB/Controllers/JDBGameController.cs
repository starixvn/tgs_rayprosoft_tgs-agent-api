using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.JDB.Services;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.Habanero.Incoming;
using TGS.Shared.Game.Models.JDB.Enums;
using TGS.Shared.Game.Models.JDB.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Controllers
{
    [Route("api/provider")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.JDB)]
    public sealed class JDBGameController : ControllerBase
    {
        private static readonly string GameProviderCode = Consts.GameProvider.JDB;

        private readonly IReadOnlyDictionary<ActionType, Func<BetData, Task<BetResult>>> _actionMapper;
        private readonly IGamePlayActions _actions;
        private readonly IJDBActionModelMapper _map;
        private readonly IJDBServices _services;
        private readonly ISettingOperations _settingOperations;

        public JDBGameController(
            IGamePlayActions actions,
            IJDBServices services,
            IJDBActionModelMapper map,
            ISettingOperations settingOperations)
        {
            _actions = actions;
            _services = services;
            _map = map;
            _settingOperations = settingOperations;

            _actionMapper = new Dictionary<ActionType, Func<BetData, Task<BetResult>>>
            {
                {ActionType.PlaceAndSettleBet, actions.PlaceAndSettleBetAsync},
                {ActionType.CancelTransaction, actions.CancelTransactionAsync}
            };
        }

        [HttpPost]
        [Route("transfer")]
        public BaseResponse TransferFundsAsync(BaseRequest request)
        {
            return new BaseResponse();
        }
        
    }
}