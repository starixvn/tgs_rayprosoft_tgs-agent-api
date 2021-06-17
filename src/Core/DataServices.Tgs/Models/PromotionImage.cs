using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PromotionImage
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public PlatformType PlatformType { get; set; }
        public Guid PromotionId { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}