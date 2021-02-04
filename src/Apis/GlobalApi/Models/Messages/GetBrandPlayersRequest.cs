using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.GlobalApi.Models.Data;
using TGS.Core.Models.Enums;

namespace TGS.Apis.GlobalApi.Models.Messages
{
    public record GetBrandPlayersRequest : PageableRequest, IRequest<PagedResponse<BrandPlayersResult>>
    {
        [BindProperty(Name = "brandcode", SupportsGet = true)]
        public string BrandCode { get; init; }

        [BindProperty(Name = "currency", SupportsGet = true)]
        public string CurrencyCode { get; init; }

        [BindProperty(Name = "playertype", SupportsGet = true)]
        public PlayerType PlayerType { get; init; }
    }
}