using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.GameAction
{
    public class GetGameActionsRequest : PageableRequest, IRequest<PagedResponse<GameActionModel>>
    {
        public Guid RoundId { get; set; }
    }
}