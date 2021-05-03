using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PromotionBonus
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public Guid PromotionId { get; set; }
        public virtual Promotion Promotion { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public string PromotionType { get; set; }
        public decimal TotalAmountLimit { get; set; }
        public GameType? GameType { get; set; }
        public decimal Percentage { get; set; }
        public decimal BonusAmountLimit { get; set; }
        public decimal TurnoverMultiple { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public virtual ICollection<PromotionPlayer> PromotionPlayers { get; set; }
    }
}