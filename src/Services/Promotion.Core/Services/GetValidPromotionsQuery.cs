using System;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Services
{
    public class GetValidPromotionsQuery : PageableRequest
    {
        public Guid BrandId { get; init; }

        public string? Language { get; init; }

        public string? PlayerCode { get; init; }
    }
}