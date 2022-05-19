using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.Promotion.Clients;
using TGS.Apis.BackendOffice.Controllers.Messages.Promotion;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Promotion
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : BaseContextController
    {
        private readonly IPromotionClient _client;

        public PromotionController(IPromotionClient client)
        {
            _client = client;
        }

        [HttpPut("ChangeOrders", Name = "ChangeOrder")]
        [Description("Change Promotion Display Order")]
        public async Task<MessageResponse> ChangeOrderAsync(ChangeOrderRequest request)
        {
            var promotionRequest = new Tgs.Promotion.Clients.Models.Promotion.ChangeOrderRequest
            {
                Orders = request.Orders.Select(x => new Tgs.Promotion.Clients.Models.Promotion.ChangeOrderModel
                {
                    Id = x.Id,
                    DisplayOrder = x.DisplayOrder
                }).ToArray()
            };

            var response = await _client.ChangeOrderAsync(promotionRequest);

            return new MessageResponse { Valid = response.Valid, Message = response.Message };
        }

        [HttpPost(Name = "CreatePromotion")]
        [Description("Create a promotion")]
        public async Task<MessageResponse> CreatePromotionAsync(
            [FromServices] IPromotionServiceClient promotionServiceClient,
            [FromBody] CreatePromotionRequest request)
        {
            var createInfo = new CreatePromotionMainInfo
            {
                Name = request.Name,
                EventDuring = (request.From.UtcDateTime, request.To.UtcDateTime),
                PromotionType = request.PromotionType,
                IsAutoSent = request.AutoSentPlayerBonus,
                PromotionLimitAmount = request.Unlimited ? null : request.TotalAmountLimit
            };
            var promotionId = await promotionServiceClient.CreatePromotionAsync(
                request.BrandId,
                createInfo,
                Username,
                HttpContext.RequestAborted).ConfigureAwait(false);

            var content = new PromotionContent
            {
                Content = request.Content,
                Convention = request.Terms,
                Localization = request.Localization,
                MobileBanner = request.MobileBanner,
                PersonalComputerBanner = request.DesktopBanner,
                SubSubject = request.SubSubject
            };
            await promotionServiceClient.SetPromotionContentAsync(
                promotionId,
                content,
                Username,
                HttpContext.RequestAborted).ConfigureAwait(false);

            await promotionServiceClient.SetPromotionRuleAsync(
                promotionId,
                new
                {
                    request.BonusAmountLimit,
                    request.Percentage,
                    request.TurnoverMultiple
                },
                Username,
                HttpContext.RequestAborted).ConfigureAwait(false);

            await promotionServiceClient.SetPromotionStatusAsync(
                promotionId,
                request.Status switch
                {
                    SwitchStatus.Enabled => Tgs.Promotion.Clients.PromotionStatus.Enabled,
                    SwitchStatus.Disabled => Tgs.Promotion.Clients.PromotionStatus.Disabled,
                    _ => throw new NotImplementedException()
                },
                Username,
                HttpContext.RequestAborted).ConfigureAwait(false);

            return new MessageResponse { Valid = true };
        }

        [HttpGet(Name = "GetPromotions")]
        [Description("Get promotions")]
        public async Task<PagedResponse<PromotionModel>> GetPromotionsAsync([FromQuery] GetPromotionsRequest request)
        {
            var promotionRequest = new Tgs.Promotion.Clients.Models.Promotion.GetPromotionsRequest
            {
                BrandId = request.BrandId,
                Name = request.Name,
                From = request.From,
                To = request.To,
                Status = (short?)request.Status,
                Type = request.Type
            };

            var response = await _client.GetPromotionsAsync(promotionRequest);

            return new PagedResponse<PromotionModel>
            {
                Valid = response.Valid,
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items.Select(x => new PromotionModel
                {
                    AmountUsed = x.AmountUsed,
                    AutoSentPlayerBonus = x.AutoSentPlayerBonus,
                    BonusAmountLimit = x.BonusAmountLimit,
                    BrandId = x.BrandId,
                    Content = x.Content,
                    CreatedByUsername = x.CreatedByUsername,
                    CreatedOn = x.CreatedOn,
                    DesktopBanner = x.DesktopBanner,
                    From = x.From,
                    GameProviderIds = x.GameProviderIds,
                    GameType = (GameType?)x.GameType,
                    Id = x.Id,
                    Localization = x.Localization,
                    MobileBanner = x.MobileBanner,
                    Name = x.Name,
                    Percentage = x.Percentage,
                    PromotionType = x.PromotionType,
                    Status = (SwitchStatus)x.Status,
                    SubSubject = x.SubSubject,
                    Terms = x.Terms,
                    To = x.To,
                    TotalAmountLimit = x.TotalAmountLimit,
                    TurnoverMultiple = x.TurnoverMultiple,
                    Unlimited = x.Unlimited,
                    UpdatedByUsername = x.UpdatedByUsername,
                    UpdatedOn = x.UpdatedOn,
                    VipLevels = x.VipLevels,
                    Number = x.Number
                }).ToArray()
            };
        }

        [HttpPut("{id}", Name = "UpdatePromotion")]
        [Description("Update a promotion")]
        public async Task<MessageResponse> UpdatePromotionAsync(Guid id, UpdatePromotionRequest request)
        {
            var promotionRequest = new Tgs.Promotion.Clients.Models.Promotion.UpdatePromotionRequest
            {
                Id = id,
                Name = request.Name,
                From = request.From,
                To = request.To,
                Status = (short)request.Status,
                PromotionType = request.PromotionType,
                BonusAmountLimit = request.BonusAmountLimit,
                Unlimited = request.Unlimited,
                GameType = (short?)request.GameType,
                Percentage = request.Percentage,
                TotalAmountLimit = request.TotalAmountLimit,
                TurnoverMultiple = request.TurnoverMultiple,
                GameProviderIds = request.GameProviderIds,
                VipLevels = request.VipLevels,
                EditorUsername = Username,
                AutoSentPlayerBonus = request.AutoSentPlayerBonus,
                Localization = request.Localization,
                SubSubject = request.SubSubject,
                Content = request.Content,
                Terms = request.Terms,
                DesktopBanner = request.DesktopBanner,
                MobileBanner = request.MobileBanner
            };

            var response = await _client.UpdatePromotionAsync(promotionRequest);
            return new MessageResponse { Valid = response.Valid, Message = response.Message };
        }

        [HttpPut("{promotionId:guid}/{status}", Name = "UpdatePromotionStatus")]
        [Description("change a Promotion's status")]
        public async Task<MessageResponse> UpdatePromotionStatusAsync(Guid promotionId, Core.Models.Enums.PromotionStatus status)
        {
            var response = await _client.ChangePromotionStatusAsync(new(promotionId, (short)status));
            return new MessageResponse { Valid = response.Valid, Message = response.Message };
        }
    }
}
