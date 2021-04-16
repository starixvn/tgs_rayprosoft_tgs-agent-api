using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PromotionPlayer
    {
        public Guid PromotionId { get; set; }
        public string PlayerCode { get; set; }
        public virtual Promotion Promotion { get; set; }
        public virtual Player Player { get; set; }
    }
}