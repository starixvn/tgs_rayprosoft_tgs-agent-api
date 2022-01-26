using System;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.GameAction
{
    public class GetGameActionsReportRequest : PageableRequest
    {
        public Guid RoundId { get; init; }
    }
}