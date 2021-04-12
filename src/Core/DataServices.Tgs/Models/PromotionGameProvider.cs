using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PromotionGameProvider
    {
        public Guid PromotionId { get; set; }
        public Guid GameProviderId { get; set; }
        public virtual Promotion Promotion { get; set; }
        public virtual GameProvider GameProvider { get; set; }
    }
}