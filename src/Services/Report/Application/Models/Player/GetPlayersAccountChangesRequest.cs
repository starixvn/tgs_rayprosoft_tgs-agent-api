using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Player
{
    public class GetPlayersAccountChangesRequest : PageableRequest, IRequest<PagedResponse<PlayersAccountChangeModel>>
    {
        public Guid BrandId { get; set; }
        public string Username { get; set; }
        public TransactionType? TransactionType { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}