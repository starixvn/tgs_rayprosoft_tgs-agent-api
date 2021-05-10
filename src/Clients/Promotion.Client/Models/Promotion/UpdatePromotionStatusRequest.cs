using System;
using TGS.Core.Models.Enums;

namespace TGS.Clients.Promotion.Client.Models.Promotion
{
    public record UpdatePromotionStatusRequest(Guid PromotionId, SwitchStatus Status);
}