using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class DistributeRegisterEvent
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public Guid PromotionId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string PlayerCode { get; set; }
        public decimal Bonus { get; set; }
    }
}
