using System;

namespace TGS.Clients.Promotion.Client.Models.Promotion
{
    public record JoinPromotionRequest(Guid PromotionId, string PlayerCode);
}