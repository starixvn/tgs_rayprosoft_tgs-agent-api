using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.Promotion;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using Tgs.Promotion.Clients;
using PromotionStatus = TGS.Core.Models.Enums.PromotionStatus;

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

            return new MessageResponse{Valid= response.Valid,Message= response.Message };
        }

        [HttpPost(Name = "CreatePromotion")]
        [Description("Create a promotion")]
        public async Task<MessageResponse> CreatePromotionAsync(CreatePromotionRequest request)
        {
            var promotionRequest = new Tgs.Promotion.Clients.Models.Promotion.CreatePromotionRequest
            {
                BrandId = request.BrandId,
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

            var response = await _client.CreatePromotionAsync(promotionRequest);
            return new MessageResponse { Valid = response.Valid, Message = response.Message };
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
                Type = request.Type,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
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
        public async Task<MessageResponse> UpdatePromotionStatusAsync(Guid promotionId, PromotionStatus status)
        {
            var response = await _client.ChangePromotionStatusAsync(new(promotionId, (short)status));
            return new MessageResponse { Valid = response.Valid, Message = response.Message };
        }

    }
}
