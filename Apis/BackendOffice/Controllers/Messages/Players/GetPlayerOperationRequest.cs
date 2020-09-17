using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Players
{
    public class GetPlayerOperationRequest : PageableRequest
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}
