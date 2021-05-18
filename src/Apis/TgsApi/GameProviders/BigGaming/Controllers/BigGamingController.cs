using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.AsiaGaming.Enums;
using TGS.Shared.Game.Models.AsiaGaming.Incoming;
using TGS.Shared.Game.Models.BigGaming.Common;
using TGS.Shared.Game.Models.BigGaming.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Controllers
{
    [Route("api/bg")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.BigGaming)]
    public class BigGamingController
    {
        private readonly ISettingOperations _settingOperations;
        private readonly IHashProvider _hashProvider;
        private readonly IPlayerOperations _playerOperations;

        public BigGamingController(
            ISettingOperations settingOperations,
            IHashProvider _hashProvider,
            IPlayerOperations playerOperations
            )
        {
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _hashProvider = _hashProvider ?? throw new ArgumentNullException(nameof(_hashProvider));
        }

        [HttpPost("entrance")]
        public async Task<BGBaseResponse> EntranceAsync(
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            BGBaseRequest baseRequest)
        {

            return baseRequest.Method.ToLower() switch
            {
                "open.operator.user.balance" => await BalanceAsync(jsonOptionsAccessor, baseRequest),
                "open.operator.order.transfer" => await BetAsync(jsonOptionsAccessor, baseRequest),
                "open.operator.calc.transfer" => await SettleAsync(jsonOptionsAccessor, baseRequest),
                _ => new BGBaseResponse()
                {
                    Id = baseRequest.Id,
                    Result = null,
                    Error = null,
                    Jsonrpc = baseRequest.Jsonrpc
                }
            };


            //return new BGBaseResponse()
            //{
            //    Id = baseRequest.Id,
            //    Result = null,
            //    Error = null,
            //    Jsonrpc = baseRequest.Jsonrpc
            //};

        }

        [HttpPost("balance")]
        public async Task<BGBaseResponse> BalanceAsync(
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            BGBaseRequest baseRequest)
        {

            var data = JsonSerializer.Deserialize<BGBalanceRequest>(JsonSerializer.Serialize(baseRequest.Params), jsonOptionsAccessor.Value.JsonSerializerOptions);

            var walletId = await _playerOperations.GetDefaultWalletIdByPlayerCodeAsync(data.LoginId).ConfigureAwait(false);

            var balance = await _playerOperations.GetPlayerBalanceAsync(walletId, false).ConfigureAwait(false);


            return new BGBaseResponse()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(balance)),
                Error = null,
                Jsonrpc = baseRequest.Jsonrpc
            };
        }

        [HttpPost("bet")]
        public async Task<BGBaseResponse> BetAsync(
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            BGBaseRequest baseRequest)
        {
            BGBetRequest data = JsonSerializer.Deserialize<BGBetRequest>(baseRequest.Params.ToString());

            var result = new BGBetResponse()
            {
                UserId = data.UserId,
                Sn = data.Sn,
                AvailableAmount = new Random().Next(1, 100),
                OrderResult = "1",
                TranId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
            };

            return new BGBaseResponse()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = null,
                Jsonrpc = baseRequest.Jsonrpc
            };
        }

        [HttpPost("settle")]
        public async Task<BGBaseResponse> SettleAsync(
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            BGBaseRequest baseRequest)
        {
            BGSettleRequest data = JsonSerializer.Deserialize<BGSettleRequest>(baseRequest.Params.ToString());

            var result = new BGSettleResponse()
            {
                UserId = data.UserId,
                Sn = data.Sn,
                AvailableAmount = new Random().Next(1, 100),
                OrderResult = "1",
                TranId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
            };

            return new BGBaseResponse()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = null,
                Jsonrpc = baseRequest.Jsonrpc
            };
        }
    }
}
