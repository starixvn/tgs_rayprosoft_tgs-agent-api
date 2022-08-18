#nullable enable

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.DepositAccumulateRoulette.Clients;
using Tgs.ServiceUtility;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator.ApplicationServices.Promotions;

namespace TGS.Apis.OperatorApi.Controllers;

[Route("api/dar")]
[ApiController]
public class DepositAccumulateRouletteController : ControllerBase
{
    [HttpGet("setting/{promotionId}", Name = "GetDepositAccumulateRouletteSetting")]
    [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async ValueTask<DepositAccumulateRouletteSettingViewModel> GetDepositAccumulateRouletteSettingAsync(
        [FromServices] IGetService<GetDepositAccumulateRouletteSettingQuery, RouletteSetting?> rouletteService,
        Guid promotionId)
    {
        var query = new GetDepositAccumulateRouletteSettingQuery(promotionId);

        var setting = await rouletteService
            .GetAsync(query, HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new DepositAccumulateRouletteSettingViewModel
        {
            RouletteItems = setting is null
                ? Array.Empty<DepositAccumulateRouletteItemViewModel>()
                : setting.Items.Select(item => new DepositAccumulateRouletteItemViewModel
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
}
