using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client.Models.Round
{
    public class GetRoundsRequest : PageableRequest
    {
        public Guid BrandId { get; set; }
        public string Username { get; set; }
        public Guid? RoundId { get; set; }
        public Guid? GameProviderId { get; set; }
        public ClientRoundStatus? Status { get; set; }
        public GameType? GameType { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}