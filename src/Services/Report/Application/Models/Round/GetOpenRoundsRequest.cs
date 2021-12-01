using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Round
{
    public class GetOpenRoundsRequest : PageableRequest, IRequest<PagedResponse<OpenRoundModel>>
    {
        public string ExternalRoundId { get; set; }

        public Guid? GameProviderId { get; set; }

        public DateTimeOffset From { get; set; }

        public DateTimeOffset To { get; set; }
    }
}