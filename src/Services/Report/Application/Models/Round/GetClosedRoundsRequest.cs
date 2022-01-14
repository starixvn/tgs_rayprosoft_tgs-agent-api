using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Round
{
    public class GetClosedRoundsRequest : PageableRequest, IRequest<PagedResponse<ClosedRoundModel>>
    {
        public string ExternalRoundId { get; set; }

        public Guid? GameProviderId { get; set; }
    }
}
