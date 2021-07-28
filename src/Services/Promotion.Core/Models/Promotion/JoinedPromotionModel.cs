using System;

namespace TGS.Services.Promotion.Models.Promotion
{
    public class JoinedPromotionModel
    {
        public DateTimeOffset CreatedOn { get; set; }

        public string PromotionName { get; set; } = default!;
    }
}
