using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Shared.Game.ApiClients.KY;
using TGS.Shared.Game.Models.KY.Commons;
using TGS.Shared.Game.Models.KY.Defines;
using TGS.Shared.Game.Models.KY.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.KY.Controllers
{
    [Route("api/ky")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.KY)]
    public class KYController : ControllerBase
    {
        private static readonly string GameProviderCode = Consts.GameProvider.KY;
        private readonly IKYApiClient _client;
        private readonly ISettingOperations _settingOperations;
        private readonly IHashProvider _hashProvider;

        public KYController(
            IKYApiClient client,
            ISettingOperations settingOperations,
            IHashProvider hashProvider
          )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
        }

        [HttpPost("GameRecord")]
        public async ValueTask<List<RoundResult>> GameRecordAsync(KYGameRecord request)
        {

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts
                .GameProviderConfigurations.KY);
            configuration.MerchantCodes.TryGetValue(request.CurrencyCode, out string agentId);

            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var timestamp = date.ToUnixTimeMilliseconds();

            var data = new KYGameRecordRequest()
            {
                Action = (int)KYAction.GameRecord,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
            };

            var dataPath = KYUtility.GetPath(data);
            var param = KYUtility.AesEncrypt(dataPath, configuration.Deskey, true);
            var plainText = $"{agentId}{timestamp}{configuration.Md5key}";
            var key = _hashProvider.Hash(plainText, HashAlgorithmKind.Md5);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            var response = await _client.GetAPIAsync<KYGameRecordResponses>(configuration.ApiRecordUrl, apiKYRequest);

            var list = default(List<RoundResult>);
            if (response.Data.Code == 0)
            {
                list = new List<RoundResult>();
                for (int i = 0; i < response.Data.Count; i++)
                {
                    var roundGameActions = new List<RoundGameActions>();
                    roundGameActions.Add(new RoundGameActions()
                    {
                        ExternalTransactionId = response.Data.List.GameID[i],
                        Type = GameActionType.PlaceBet,
                        Amount = Convert.ToDecimal(response.Data.List.AllBet[i]) * -1,
                        Turnover = 0,
                        WinLoss = 0,
                        UnsettledBets = Convert.ToDecimal(response.Data.List.AllBet[i]) * -1,
                        Balance = 0,
                        BetType = default(string),
                        ExternalBetId = response.Data.List.GameID[i],
                        ExternalGameActionGroupId = response.Data.List.GameID[i],
                        ReferencedGameActionId = default(Guid),
                        GameActionBatchId = default(Guid),
                        RecordedOn = DateTimeOffset.UtcNow,
                        SentOn = DateTimeOffset.UtcNow,
                        IsClosingRound = false,
                        ClosingRoundExplicitRequest = null,
                        PlayerIp = string.Empty,
                        ExtraData = string.Empty,
                        JackpotContribution = string.Empty,
                    });

                    roundGameActions.Add(new RoundGameActions()
                    {
                        ExternalTransactionId = response.Data.List.GameID[i],
                        Type = GameActionType.LoseBet,
                        Amount = Convert.ToDecimal(response.Data.List.AllBet[i]),
                        Turnover = Convert.ToDecimal(response.Data.List.CellScore[i]),
                        WinLoss = Convert.ToDecimal(response.Data.List.Profit[i]),
                        UnsettledBets = 0,
                        Balance = 0,
                        BetType = default(string),
                        ExternalBetId = response.Data.List.GameID[i],
                        ExternalGameActionGroupId = response.Data.List.GameID[i],
                        ReferencedGameActionId = default(Guid),
                        GameActionBatchId = default(Guid),
                        RecordedOn = DateTimeOffset.UtcNow,
                        SentOn = DateTimeOffset.UtcNow,
                        IsClosingRound = false,
                        ClosingRoundExplicitRequest = null,
                        PlayerIp = string.Empty,
                        ExtraData = string.Empty,
                        JackpotContribution = string.Empty,
                    });

                    list.Add(new RoundResult()
                    {
                        ExternalRoundId = response.Data.List.GameID[i],
                        PlayerCode = response.Data.List.Accounts[i].Split('_')[1],
                        GameId = Convert.ToInt32(response.Data.List.KindID[i]),
                        CurrencyCode = request.CurrencyCode,
                        CreatedOn = DateTimeOffset.UtcNow,
                        ClosedOn = DateTimeOffset.UtcNow,
                        UpdatedOn = DateTimeOffset.UtcNow,
                        ExternalCreatedOn = DateTimeOffset.Parse(response.Data.List.GameStartTime[i]),
                        ExternalClosedOn = DateTimeOffset.Parse(response.Data.List.GameEndTime[i]),
                        BetAmount = Convert.ToDecimal(response.Data.List.AllBet[i]),
                        ValidBetAmount = Convert.ToDecimal(response.Data.List.CellScore[i]),
                        WinLoss = Convert.ToDecimal(response.Data.List.Profit[i]),
                        Commission = Convert.ToDecimal(response.Data.List.Revenue[i]),
                        Turnover = Convert.ToDecimal(response.Data.List.CellScore[i]),
                        SettleAmount = Convert.ToDecimal(response.Data.List.AllBet[i]) + Convert.ToDecimal(response.Data.List.Profit[i]),
                        TieBetAdjustment = null,
                        RoundGameActions = roundGameActions,
                    });
                }
            }

            return list;
        }
    }

    public record KYGameRecord
    {
        public string CurrencyCode { get; init; }
        public long StartTime { get; init; }
        public long EndTime { get; init; }
    }

    public record RoundGameActions
    {
        public string ExternalTransactionId { get; set; }
        public GameActionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal Turnover { get; set; }
        public decimal WinLoss { get; set; }
        public decimal UnsettledBets { get; set; }
        public decimal Balance { get; set; }
        public string BetType { get; set; }
        public string ExternalBetId { get; set; }
        public string ExternalGameActionGroupId { get; set; }
        public Guid? ReferencedGameActionId { get; set; }
        public Guid? GameActionBatchId { get; set; }
        public DateTimeOffset RecordedOn { get; set; }
        public DateTimeOffset? SentOn { get; set; }
        public bool IsClosingRound { get; set; }
        public bool? ClosingRoundExplicitRequest { get; set; }
        public string PlayerIp { get; set; }
        public string ExtraData { get; set; }
        public string JackpotContribution { get; init; }

    }

    public record RoundResult
    {
        public Guid RoundId { get; set; }
        public string ExternalRoundId { get; set; }
        public string PlayerCode { get; set; }
        public int GameId { get; set; }
        public string CurrencyCode { get; set; }
        public RoundStatus Status { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? ClosedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public DateTimeOffset? ExternalCreatedOn { get; set; }
        public DateTimeOffset? ExternalClosedOn { get; set; }
        public decimal? BetAmount { get; set; }
        public decimal? ValidBetAmount { get; set; }
        public decimal? SettleAmount { get; set; }
        public decimal? Commission { get; set; }
        public decimal? Turnover { get; set; }
        public decimal? WinLoss { get; set; }
        public decimal? TieBetAdjustment { get; set; }
        public virtual RoundDetailResult RoundDetail { get; set; }
        public virtual List<RoundGameActions> RoundGameActions { get; set; }
    }

    public record RoundDetailResult
    {
        public string AdditionalData { get; set; }
    }
}
