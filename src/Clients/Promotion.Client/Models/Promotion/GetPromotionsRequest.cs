using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Promotion.Client.Models.Promotion
{
    public class GetPromotionsRequest : PageableRequest
    {
        public Guid BrandId { get; set; }
        public string PlayerCode { get; set; }
        public string Type { get; set; }
        public SwitchStatus? Status { get; set; }
        public string Name { get; set; }
    }
}