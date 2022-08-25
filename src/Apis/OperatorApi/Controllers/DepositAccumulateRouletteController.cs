#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator.ApplicationServices.Promotions;
using RouletteResult = Tgs.DepositAccumulateRoulette.Clients.RouletteResult;

namespace TGS.Apis.OperatorApi.Controllers;

[Route("api/dar")]
[ApiController]
public class DepositAccumulateRouletteController : ControllerBase
{
    [HttpGet("setting/{promotionId}", Name = "GetDepositAccumulateRouletteSetting")]
    [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async ValueTask<DepositAccumulateRouletteSettingViewModel> GetDepositAccumulateRouletteSettingAsync(
        [FromServices] IGetService<GetPromotionRuleQuery, JsonDocument?> getRuleService,
        Guid promotionId)
    {
        var query = new GetPromotionRuleQuery(promotionId);

        var campaignDetail = await getRuleService
            .GetAsync(query, HttpContext.RequestAborted)
            .ConfigureAwait(false);

        if (campaignDetail is null)
        {
            return new DepositAccumulateRouletteSettingViewModel
            {
                RouletteItems = Array.Empty<DepositAccumulateRouletteItemViewModel>()
            };
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var setting = campaignDetail.Deserialize<RouletteSetting>(jsonOptions);

        return new DepositAccumulateRouletteSettingViewModel
        {
            RouletteItems = setting is null ?
                Array.Empty<DepositAccumulateRouletteItemViewModel>() :
                setting.RouletteItems.Select(item => new DepositAccumulateRouletteItemViewModel
                {
                    Title = item.Title
                })
        };
    }

    [HttpPost("{promotionId}", Name = "PlayDepositAccumulateRoulette")]
    [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async ValueTask<DepositAccumulateRouletteResultViewModel> PlayDepositAccumulateRouletteAsync(
        [FromServices] IGetService<PlayDepositAccumulateRouletteCommand, RouletteResult> rouletteService,
        Guid promotionId)
    {
        var playerTokenData = HttpContext.Items.GetPlayerTokenData();
        var playerCode = playerTokenData.PlayerCode;

        var command = new PlayDepositAccumulateRouletteCommand(promotionId, playerCode);

        var result = await rouletteService
            .GetAsync(command, HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new DepositAccumulateRouletteResultViewModel
        {
            ItemIndex = result.ItemIndex,
            RouletteCount = result.RemainingCount
        };
    }

    private class RouletteItem
    {
        public string Id { get; init; } = default!;

        public string Title { get; init; } = default!;
    }

    private class RouletteSetting
    {
        public IEnumerable<RouletteItem> RouletteItems { get; init; } = Array.Empty<RouletteItem>();
    }
}
