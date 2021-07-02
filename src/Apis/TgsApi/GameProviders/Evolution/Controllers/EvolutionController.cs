using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Core.Models;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.Evolution.Commons;
using TGS.Shared.Game.Models.Evolution.Incoming;
using TGS.Core.Infrastructures.Extensions;
using TGS.Apis.TgsApi.GameProviders.Evolution.Services;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Models.Enums;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Controllers
{
    [Route("evo/api/")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.Evolution)]
    public sealed class EvolutionController : ControllerBase
    {
        private readonly IGamePlayActions _actions;
        private readonly IEvolutionActionModelMapper _map;
        private readonly IEvolutionServices _services;

        public EvolutionController(
            IGamePlayActions actions,
            IEvolutionActionModelMapper map,
            IEvolutionServices services
        )
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        [HttpPost("check")]
        public async Task<EvolutionCheckUserResponse> GetCheckAsync(
            EvolutionCheckUserRequest source)
        {
            return new EvolutionCheckUserResponse()
            {
                Status = "OK",
                Sid = source.Sid,
                Uuid = source.Uuid
            };
        }

        [HttpPost("balance")]
        public async Task<EvolutionStandardResponse> GetBalanceAsync(
           EvolutionBalanceRequest source)
        {
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.Evolution, source.UserId).ConfigureAwait(false);
            return new EvolutionStandardResponse()
            {
                Status = "OK",
                Balance = getBalance.Balance.Truncate(2),
                Uuid = source.Uuid
            };
        }

        [HttpPost("debit")]
        public async Task<EvolutionStandardResponse> BetAsync(
            EvolutionDebitRequest source)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = _map.Bet(gameProviderConfigurationData, playerCode, source);
            var betResult = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            return new EvolutionStandardResponse()
            {
                Status = "OK",
                Balance = betResult.Balance.Truncate(2),
                Uuid = source.Uuid
            };
        }

        [HttpPost("credit)")]
        public async Task<EvolutionStandardResponse> SettleAsync(
          EvolutionCreditRequest source)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = _map.Settle(gameProviderConfigurationData, playerCode, source);
            var betResult = await _actions.SettleBetAsync(betData).ConfigureAwait(false);

            return new EvolutionStandardResponse()
            {
                Status = "OK",
                Balance = betResult.Balance.Truncate(2),
                Uuid = source.Uuid
            };
        }

        [HttpPost("cancel")]
        public async Task<EvolutionStandardResponse> CancelAsync(
            EvolutionCancelRequest source)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = await _map.CancelAsync(gameProviderConfigurationData, playerCode, source).ConfigureAwait(false);
            var betResult = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);

            return new EvolutionStandardResponse()
            {
                Status = "OK",
                Balance = betResult.Balance.Truncate(2),
                Uuid = source.Uuid
            };
        }

        [HttpPost("sid")]
        public async Task<EvolutionCheckUserResponse> SidAsync(
            EvolutionCheckUserRequest source)
        {
            return new EvolutionCheckUserResponse()
            {
                Status = "OK",
                Sid = source.Sid,
                Uuid = source.Uuid
            };
        }

        [HttpPost("ping")]
        public EvolutionBaseResponse Ping()
        {
            return (new EvolutionBaseResponse()
            {
                Status = "OK"
            });
        }
    }
}
