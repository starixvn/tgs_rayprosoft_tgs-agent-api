using System;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.Evolution.Commons;
using TGS.Shared.Game.Models.Evolution.Incoming;
using TGS.Core.Infrastructures.Extensions;

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
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public EvolutionController(
            IGamePlayActions actions,
            IJsonSerializationProvider jsonSerializationProvider
        )
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _jsonSerializationProvider = jsonSerializationProvider ??
                throw new ArgumentNullException(nameof(jsonSerializationProvider));


            _jsonSerializerOptions = new()
            {
                IgnoreReadOnlyProperties = true,
                WriteIndented = true,
                //略過所有預設值屬性
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };
        }

        [HttpPost("check")]
        public async Task<EvolutionCheckUserResponse> GetCheckAsync(
            EvolutionCheckUserRequest source)
        {
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.Evolution, source.UserId).ConfigureAwait(false);
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
            /*
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.Evolution, playerCode).ConfigureAwait(false);
            */
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
            /*
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.Evolution, playerCode).ConfigureAwait(false);
            */
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.Evolution, source.UserId).ConfigureAwait(false);
            return new EvolutionStandardResponse()
            {
                Status = "OK",
                Balance = getBalance.Balance.Truncate(2),
                Uuid = source.Uuid
            };
        }

        [HttpPost("credit)")]
        public async Task<EvolutionStandardResponse> SettleAsync(
          EvolutionCreditRequest source)
        {
            /*
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.Evolution, playerCode).ConfigureAwait(false);
            */
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.Evolution, source.UserId).ConfigureAwait(false);
            return new EvolutionStandardResponse()
            {
                Status = "OK",
                Balance = getBalance.Balance.Truncate(2),
                Uuid = source.Uuid
            };
        }

        [HttpPost("cancel")]
        public async Task<EvolutionStandardResponse> CancelAsync(
            EvolutionCancelRequest source)
        {
            /*
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.Evolution, playerCode).ConfigureAwait(false);
            */
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.Evolution, source.UserId).ConfigureAwait(false);
            return new EvolutionStandardResponse()
            {
                Status = "OK",
                Balance = getBalance.Balance.Truncate(2),
                Uuid = source.Uuid
            };
        }

        [HttpPost("sid")]
        public async Task<EvolutionCheckUserResponse> SidAsync(
            EvolutionCheckUserRequest source)
        {
            /*
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.Evolution, playerCode).ConfigureAwait(false);
            */
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.Evolution, source.UserId).ConfigureAwait(false);
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
