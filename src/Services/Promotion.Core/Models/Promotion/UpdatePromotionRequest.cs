using System;
using System.Text.Json;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Models.Promotion
{
    public class UpdatePromotionRequest : IRequest<MessageResponse>
    {
        public bool AutoSentPlayerBonus { get; set; }

        public decimal BonusAmountLimit { get; set; }

        public string EditorUsername { get; set; } = default!;

        public DateTimeOffset From { get; set; }

        public Guid[] GameProviderIds { get; set; } = default!;

        public GameType? GameType { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public decimal Percentage { get; set; }

        public string PromotionType { get; set; } = default!;

        public DateTimeOffset To { get; set; }

        public decimal TotalAmountLimit { get; set; }

        public decimal TurnoverMultiple { get; set; }

        public bool Unlimited { get; set; }

        public int[] VipLevels { get; set; } = default!;
        public JsonDocument? Localization { get; set; }
        public string SubSubject { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string Terms { get; set; } = default!;
        public string DesktopBanner { get; set; } = default!;
        public string MobileBanner { get; set; } = default!;
    }
}