using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client.Models.Player
{
    public class GetPlayersAccountChangesRequest : PageableRequest
    {
        public Guid BrandId { get; set; }
        public string Username { get; set; }
        public TransactionType? TransactionType { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}