using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Models.Promotion
{
    public class GetPromotionsRequest : PageableRequest, IRequest<PagedResponse<PromotionModel>>
    {
        public Guid BrandId { get; set; }

        public string Name { get; set; }

        public string PlayerCode { get; set; }

        public SwitchStatus? Status { get; set; }

        public string Type { get; set; }

        public DateTimeOffset? CurrentDate { get; set; }
        public string Language { get; set; }
    }
}