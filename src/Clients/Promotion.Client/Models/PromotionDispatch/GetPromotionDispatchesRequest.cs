using System;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Promotion.Client.Models.PromotionDispatch
{
    public class GetPromotionDispatchesRequest : PageableRequest
    {
        public Guid BrandId { get; set; }
        public string PromotionGroup { get; set; }
        public string PromotionName { get; set; }
        public string Username { get; set; }
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
    }
}