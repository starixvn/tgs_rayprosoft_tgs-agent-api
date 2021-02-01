using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Round
{
    public class GetRoundsRequest : PageableRequest, IRequest<PagedResponse<RoundModel>>
    {
        public Guid BrandId { get; set; }
        public string PlayerName { get; set; }
        public Guid? RoundId { get; set; }
        public Guid? GameProviderId { get; set; }
        public RoundStatus? Status { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}