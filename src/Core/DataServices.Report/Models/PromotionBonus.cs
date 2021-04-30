using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Report.Models
{
    public class PromotionBonus
    {
        public Guid Id { get; set; }
        public Guid PromotionId { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public string PromotionType { get; set; }
        public decimal TotalAmountLimit { get; set; }
        public GameType? GameType { get; set; }
        public decimal Percentage { get; set; }
        public decimal BonusAmountLimit { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public virtual ICollection<PromotionPlayer> Players { get; set; }
    }
}