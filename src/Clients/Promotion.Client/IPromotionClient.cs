using System;
using System.Threading.Tasks;
using TGS.Clients.Promotion.Client.Models.Promotion;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Promotion.Client
{
    public interface IPromotionClient
    {
        Task<MessageResponse> ChangePromotionStatusAsync(UpdatePromotionStatusRequest request);

        Task<bool> CheckCanJoinDepositPromotionAsync(Guid brandId, string playerCode, Guid promotionId);

        Task<MessageResponse> CreatePromotionAsync(CreatePromotionRequest request);

        Task<PagedResponse<PromotionModel>> GetPromotionsAsync(GetPromotionsRequest request);

        Task<MessageResponse> JoinAsync(JoinPromotionRequest request);

        Task<MessageResponse> UpdatePromotionAsync(UpdatePromotionRequest request);
    }
}
