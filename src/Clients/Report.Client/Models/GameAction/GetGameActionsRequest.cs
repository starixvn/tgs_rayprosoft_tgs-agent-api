using System;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client.Models.GameAction
{
    public class GetGameActionsRequest : PageableRequest
    {
        public Guid RoundId { get; set; }
    }
}