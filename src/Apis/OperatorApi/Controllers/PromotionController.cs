using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.Promotion.Clients;
using Tgs.Promotion.Clients.Models.Promotion;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class PromotionController : ControllerBase
{
    private readonly IPromotionClient _promotionClient;

    public PromotionController(IPromotionClient promotionClient)
    {
        _promotionClient = promotionClient ?? throw new ArgumentNullException(nameof(promotionClient));
    }

    [HttpGet("{language}", Name = "GetPromotions")]
    [Description("Get promotions")]
    [Consumes(MediaTypeNames.Application.Json)]
    [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
    [Obsolete("預計淘汰")]
    public async ValueTask<PagingResponse<PromotionResponse>> GetPromotionsAsync(string language,
        [FromQuery] PagingRequest request)
    {
        var brandId = HttpContext.Items.GetBrandId();

        var promotionsRequest = new GetValidPromotionsRequest
        {
            BrandId = brandId,
            Language = language,
            Page = request.Page,
            Rows = request.Rows,
            OrderBy = request.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
            SortBy = "Number",
            ShowAll = request.ShowAll
        };
        var response = await _promotionClient.GetValidPromotionsAsync(promotionsRequest);

        return new PagingResponse<PromotionResponse>
        {
            TotalCount = response.TotalCount,
            Items = response.Items.Select(item => new PromotionResponse
            {
                AmountUsed = item.AmountUsed,
                AutoSentPlayerBonus = item.AutoSentPlayerBonus,
                BonusAmountLimit = item.BonusAmountLimit,
                BrandId = item.BrandId,
                Content = item.Content,
                CreatedByUsername = item.CreatedByUsername,
                CreatedOn = item.CreatedOn,
                DesktopBanner = item.DesktopBanner,
                From = item.From,
                GameProviderIds = item.GameProviderIds,
                GameType = (GameType?)item.GameType,
                Id = item.Id,
                Localization = item.Localization?.ContainsKey(language) == true
                    ? item.Localization[language]
                    : JsonDocument.Parse("{}"),
                MobileBanner = item.MobileBanner,
                Name = item.Name,
                Percentage = item.Percentage,
                PromotionType = item.PromotionType,
                Status = (SwitchStatus)item.Status,
                SubSubject = item.SubSubject,
                Terms = item.Terms,
                To = item.To,
                TotalAmountLimit = item.TotalAmountLimit,
                TurnoverMultiple = item.TurnoverMultiple,
                Unlimited = item.Unlimited,
                UpdatedByUsername = item.UpdatedByUsername,
                UpdatedOn = item.UpdatedOn,
                VipLevels = item.VipLevels
            })
        };
    }

    [HttpPost("{promotionId}/join", Name = "JoinPromotion")]
    [Description("Join a promotion")]
    [Consumes(MediaTypeNames.Application.Json)]
    [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async ValueTask<JoinPromotionResponse> JoinPromotionAsync(Guid promotionId)
    {
        var playerTokenData = HttpContext.Items.GetPlayerTokenData();
        var response = await _promotionClient.JoinAsync(new(promotionId, playerTokenData.PlayerCode));

        return new() { Success = response.Valid, ErrorMessage = !response.Valid ? response.Message : string.Empty };
    }

    [HttpGet(Name = "ListPromotions")]
    [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
    public IAsyncEnumerable<PromotionViewModel> ListPromotionsAsync(
        [FromServices] IPromotionServiceClient promotionServiceClient,
        [FromServices] IOptions<JsonOptions> jsonOptionsAccessor)
    {
        var brandId = HttpContext.Items.GetBrandId();
        var playerTokenData = HttpContext.Items.GetPlayerTokenData();

        return (playerTokenData is null
            ? promotionServiceClient
                .ListPromotionsAsync(brandId, HttpContext.RequestAborted)
            : promotionServiceClient
                .ListPromotionsAsync(brandId, playerTokenData.PlayerCode, HttpContext.RequestAborted))
                .Select(promotion => new PromotionViewModel
                {
                    Id = promotion.Id,
                    Name = promotion.Name,
                    From = promotion.EventDuring.From,
                    To = promotion.EventDuring.To,
                    PromotionType = promotion.PromotionType,
                    Content = promotion.Content,
                    DesktopBanner = promotion.PersonalComputerBanner,
                    MobileBanner = promotion.MobileBanner,
                    Localization = promotion.Localization,
                    SubSubject = promotion.SubSubject,
                    Terms = promotion.Convention,
                    VipLevels = promotion.VipLevels,
                    CampaignDetail = promotion.PromotionType == "Roulette"
                        ? JsonSerializer.SerializeToDocument(
                            promotion.CampaignDetail.Deserialize<RouletteSettingsViewModel>(jsonOptionsAccessor.Value.JsonSerializerOptions)!,
                            jsonOptionsAccessor.Value.JsonSerializerOptions)
                        : promotion.CampaignDetail
                });
    }
}
