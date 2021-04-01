using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Player
{
    public class GetPlayerContributionsRequest : PageableRequest, IRequest<PagedResponse<PlayerContributionModel>>
    {
        public Guid BrandId { get; set; }
        public string Username { get; set; }
        public int? Level { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}