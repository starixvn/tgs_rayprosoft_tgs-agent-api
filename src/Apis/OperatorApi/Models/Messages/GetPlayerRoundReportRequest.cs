using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public class GetPlayerRoundReportRequest : PageableRequest, IRequest<PagedResponse<PlayerRoundReportModel>>
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}