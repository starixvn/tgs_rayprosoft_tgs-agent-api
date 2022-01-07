using System;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using Tgs.Logging;
using Tgs.MessageQueue;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Models;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Services;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.WMCasino;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.WMCasino.Enums;
using TGS.Shared.Game.Models.WMCasino.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Controllers
{
    [Route("api/wm")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.WMCasino)]
    public class WMCasinoController : ControllerBase
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<WMCasinoController> _logger;

        public WMCasinoController(
            ILogger<WMCasinoController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = false
            };
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("betdetail")]
        public async Task<ErrorResponse> BetDetailsAsync(
            [FromServices] ISettingOperations settingOperations,
            [FromServices] IWMCasinoDataStore store,
            [FromServices] IWMCasinoActionModelMapper map,
            [FromServices] IMessageSender messageSender,
            BetDetailRequest request)
        {
            var agent = HttpContext.Items.Get<WMCasinoAgent>(WMCasinoConsts.ContextItems.Agent, null);

            var gameResults = request.Results;
            var roundNumber = $"{gameResults[0].ExternalGameId}_{gameResults[0].RoundId}_{gameResults[0].SubRoundId}";

            await store
                .AddBetDetailRequestInfoAsync(agent.AgentId, roundNumber, gameResults)
                .ConfigureAwait(false);

            var totalCount = await store
                .GetBetDetailRequestInfoCountAsync(agent.AgentId, roundNumber)
                .ConfigureAwait(false);

            if (totalCount != request.TotalCount)
            {
                return map.Response(GameApiError.Success, string.Empty);
            }

            var keyRequest = new BetDetailRequestInfoKey(agent.AgentId, roundNumber);
            var bytes = JsonSerializer.SerializeToUtf8Bytes(
                keyRequest,
                new JsonSerializerOptions { WriteIndented = false });

            await messageSender
                .SendAsync("tgs.gameprovider.wm.betdetailrequest.process", bytes)
                .ConfigureAwait(true);

            return map.Response(GameApiError.Success, string.Empty);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("canceltransfer")]
        public async ValueTask<CancelWalletTransferResponse> CancelWalletTransferAsync(
            [FromServices] IWMCasinoBetActionServices services,
            [FromServices] IWMCasinoActionModelMapper map,
            CancelWalletTransferRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);

            if (request.ActionType == WalletActionType.Debit)
            {
                var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);

                var gameProviderConfigurationId = HttpContext.Items.Get(
                    Consts.ContextItems.GameProviderConfigurationId,
                    Guid.Empty);

                var betResult = await services
                    .CancelLockAmountAsync(gameProviderId, gameProviderConfigurationId, playerCode, request)
                    .ConfigureAwait(false);

                return map.Response(request, betResult.Balance);
            }

            var balanceResult = await services.GetBalanceAsync(playerCode).ConfigureAwait(false);

            return map.Response(request, balanceResult.Balance);
        }

        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost("entrance")]
        public ValueTask<ErrorResponse> EntranceAsync([FromForm] BaseRequest request)
            => ValueTask.FromResult(new ErrorResponse());

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("gamerecord")]
        public ValueTask<GameProviderGameRecordResponse> GetGameRecordAsync(
            [FromServices] IWMCasinoGameProviderHistoryServices services,
            GameProviderGameRecordRequest request)
            => services.GetGameRecordAsync(request);

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("gamerecord/manual")]
        public async Task<ManuelRoundRecordResponse> ManualExecutePlayerRoundRecordAsync(
            [FromServices] IWMCasinoDataStore store,
            [FromServices] IWMCasinoBetActionServices service,
            [FromServices] ITgsLog tgsLog,
            ManuelRoundRecordRequest request)
        {
            tgsLog = tgsLog.SetLogger("wmcasino");

            var gameNumber = $"{request.ExternalGameId}_{request.ExternalRoundId}";
            var batchId = $"{request.PlayerCode}:{gameNumber}";

            var betDetails = (await store
                    .GetBetResultsByBatchIdAsync(batchId)
                    .ConfigureAwait(false))
                .ToArray();

            if (betDetails.Length == 0)
            {
                var message = $"Can't find batch with id: {batchId}";

                tgsLog.LogError(message);
                _logger.LogError(new EventId(1000), message);

                return new ManuelRoundRecordResponse
                {
                    ErrorCode = 1000,
                    ErrorMessage = message
                };
            }

            try
            {
                foreach (var betDetail in betDetails)
                {
                    await service
                        .ExecuteBetResultsByRoundAsync(
                            betDetail.GameProviderId,
                            betDetail.GameProviderConfigurationId,
                            betDetail.PlayerCode,
                            new[] { betDetail.GameBetResult })
                        .ConfigureAwait(false);
                }

                await store.RemoveBetResultsByBatchIdAsync(batchId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var value = JsonSerializer.Serialize(betDetails, _jsonSerializerOptions);
                var message = $"Fail to execute WMCasino bet result. BatchId: {batchId}, BetDetails: {value}";

                tgsLog.LogError(message, ex);
                _logger.LogError(new EventId(1100), ex, message);

                return new ManuelRoundRecordResponse
                {
                    ErrorCode = 1100,
                    ErrorMessage = message
                };
            }

            return new ManuelRoundRecordResponse
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty
            };
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("balance")]
        public async ValueTask<GetBalanceResponse> PlayersBalanceAsync(
                [FromServices] IWMCasinoBetActionServices services,
                [FromServices] IWMCasinoActionModelMapper map,
                GetBalanceRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var r = await services.GetBalanceAsync(playerCode).ConfigureAwait(false);

            return map.Response(request.User, r);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("transfer")]
        public async ValueTask<WalletTransferResponse> WalletTransferAsync(
            [FromServices] IWMCasinoBetActionServices services,
            [FromServices] IWMCasinoActionModelMapper map,
            WalletTransferRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);

            if (request.ActionType == WalletActionType.Debit)
            {
                var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);

                var gameProviderConfigurationId = HttpContext.Items.Get(
                    Consts.ContextItems.GameProviderConfigurationId,
                    Guid.Empty);

                var betResult = await services
                    .LockAmountAsync(gameProviderId, gameProviderConfigurationId, playerCode, request)
                    .ConfigureAwait(false);

                return map.Response(request, betResult.Balance);
            }

            var balanceResult = await services.GetBalanceAsync(playerCode).ConfigureAwait(false);

            return map.Response(request, balanceResult.Balance);
        }
    }
}
