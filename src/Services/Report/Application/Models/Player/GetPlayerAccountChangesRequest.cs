using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Player
{
    public class GetPlayerAccountChangesRequest : PageableRequest, IRequest<PagedResponse<PlayerAccountChangeModel>>
    {
        public Guid BrandId { get; set; }
        public string PlayerCode { get; set; }
        public AccountChangeType? AccountChangeType { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}