using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.PromotionCore.Models.Promotion
{
    public class UpdatePromotionRequest : IRequest<MessageResponse>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public SwitchStatus Status { get; set; }
        public bool AutoSentPlayerBonus { get; set; }
        public string PromotionType { get; set; }
        public decimal BonusAmountLimit { get; set; }
        public bool Unlimited { get; set; }
        public GameType? GameType { get; set; }
        public decimal Percentage { get; set; }
        public decimal TotalAmountLimit { get; set; }
        public decimal TurnoverMultiple { get; set; }
        public Guid[] GameProviderIds { get; set; }
        public int[] VipLevels { get; set; }
        public string EditorUsername { get; set; }
    }
}