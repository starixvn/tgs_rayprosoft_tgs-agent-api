using System;

namespace TGS.Services.PromotionCore.Models.PromotionBonus
{
    public class PromotionBonusPlayerModel
    {
        public int Id { get; set; }
        public string PlayerCode { get; set; }
        public string PromotionType { get; set; }
        public Guid PromotionBonusId { get; set; }
        public decimal BonusAmount { get; set; }
    }
}