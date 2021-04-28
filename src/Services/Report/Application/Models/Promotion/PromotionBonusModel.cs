using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Report.Application.Models.Promotion
{
    public class PromotionBonusModel
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public Guid PromotionId { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public bool AutoSentPlayerBonus { get; set; }
        public string PromotionType { get; set; }
        public decimal TotalAmountLimit { get; set; }
        public GameType? GameType { get; set; }
        public decimal Percentage { get; set; }
        public decimal BonusAmountLimit { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public PromotionBonusPlayerModel[] Players { get; set; }
    }
}