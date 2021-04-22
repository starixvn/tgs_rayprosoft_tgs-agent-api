using System;

namespace TGS.Services.Report.Application.Models.Promotion
{
    public class PromotionBonusPlayerModel
    {
        public int Id { get; set; }
        public string PlayerCode { get; set; }
        public string PromotionType { get; set; }
        public Guid PromotionBonusId { get; set; }
        public bool IsSent { get; set; }
        public decimal BonusAmount { get; set; }
    }
}