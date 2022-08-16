using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator;
using TGS.Services.Operator.ApplicationServices.Promotions;

namespace TGS.Apis.OperatorApi.Controllers;

[Route("api/dar")]
[ApiController]
public class DepositAccumulateRouletteController : ControllerBase
{
    [HttpPost("roulette/{promotionId}", Name = "DepositAccumulateRoulette")]
    [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<RouletteResultResponse> DepositAccumulateRouletteAsync(
        [FromServices] IGetService<DepositAccumulateRouletteCommand, RouletteResultWithCount> rouletteService,
        Guid promotionId)
    {
        var playerTokenData = HttpContext.Items.GetPlayerTokenData();
        var playerCode = playerTokenData.PlayerCode;

        var command = new DepositAccumulateRouletteCommand(promotionId, playerCode);

        var result = await rouletteService
            .GetAsync(command, HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new RouletteResultResponse
        {
            ItemIndex = result.ItemIndex,
            RouletteCount = result.RouletteCount
        };
    }

    [HttpPost("roulette/{promotionId}/settings", Name = "GetDepositAccumulateRouletteSettings")]
    [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async IAsyncEnumerable<RouletteSettingViewModel> GetDepositAccumulateRouletteSettingAsync(
        [FromServices] IQueryService<DepositAccumulateRouletteSettingQuery, RouletteItem> rouletteService,
        Guid promotionId)
    {
        var query = new DepositAccumulateRouletteSettingQuery(promotionId);

        var items = await rouletteService
            .QueryAsync(query, HttpContext.RequestAborted)
            .ToArrayAsync(HttpContext.RequestAborted)
            .ConfigureAwait(false);

        foreach (var item in items)
            yield return new RouletteSettingViewModel
            {
                Id = item.Id,
                Title = item.Title
            };
    }
}
