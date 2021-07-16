using System;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Promotion.Client.Models.Promotion
{
    public class GetJoinedPromotionsRequest : PageableRequest
    {
        public string PlayerCode { get; set; }
        public DateTimeOffset From { get; init; }
        public DateTimeOffset To { get; init; }
    }
}