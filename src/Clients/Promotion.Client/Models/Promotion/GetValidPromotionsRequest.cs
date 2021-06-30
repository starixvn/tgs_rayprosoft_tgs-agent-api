using System;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Promotion.Client.Models.Promotion
{
    public class GetValidPromotionsRequest : PageableRequest
    {
        public Guid BrandId { get; set; }
        public string PlayerCode { get; set; }
        public string Language { get; set; }
    }
}